using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace Day20
{
    

    class Program
    {
        static void Main(string[] args)
        {
            Ballot ballot = BuildBallotFromJsonWeb("https://kevinle108.github.io/JsonCollection/BALLOT_0001.json");
            //Ballot ballot = BuildBallotFromJson("BALLOT_0002.json");
            ballot.Output();
            Vote(ballot);
        }

        class BallotFromJson
        {
            public string BallotName { get; set; }
            public ContestHelper[] Contests { get; set; }

            public class ContestHelper
            {
                public string ContestCode { get; set; }
                public string[] CandidateCodes { get; set; }
            }
        }

        class ContestFromJson
        {
            public string ContestName { get; set; }
            public int MaxChoices { get; set; }
            public bool WriteIn { get; set; }
            public CandidateHelper[] Candidates { get; set; }

            public class CandidateHelper
            {
                public string CandidateCode { get; set; }
                public string CandidateName { get; set; }
                public string CandidateParty { get; set; }
            }
        }

        static Ballot BuildBallotFromJsonWeb(string url)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage webRequest = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = client.Send(webRequest);
            Stream stream = response.Content.ReadAsStream();
            StreamReader reader = new StreamReader(stream);
            string data = reader.ReadToEnd();
            BallotFromJson deserializedBallot = JsonSerializer.Deserialize<BallotFromJson>(data);
            Ballot ballot = new Ballot(deserializedBallot.BallotName);
            Contest contest;
            Candidate candidate;
            foreach (BallotFromJson.ContestHelper contestItem in deserializedBallot.Contests)
            {
                string contestData = File.ReadAllText($"CONTEST_{contestItem.ContestCode}.json");
                ContestFromJson deserializedContest = JsonSerializer.Deserialize<ContestFromJson>(contestData);
                contest = new Contest(contestItem.ContestCode, deserializedContest.ContestName, deserializedContest.MaxChoices);

                // look at the order of candidates in contestDeserialized.CandidateCodes
                foreach (string candidateCode in contestItem.CandidateCodes)
                {
                    // find the candidate with the code
                    ContestFromJson.CandidateHelper foundCandidate = deserializedContest.Candidates.First(x => x.CandidateCode == candidateCode);

                    // create and add the candidate to the contest
                    candidate = new Candidate(candidateCode, foundCandidate.CandidateName, foundCandidate.CandidateParty);
                    contest.AddCandidate(candidate);
                }

                // check if contest needs writeins
                if (deserializedContest.WriteIn) contest.AddBlankWriteIns();

                ballot.AddContest(contest);
            }
            return ballot;
        }

        static Ballot BuildBallotFromJson(string fileName)
        {

            string data = File.ReadAllText(fileName);
            BallotFromJson deserializedBallot = JsonSerializer.Deserialize<BallotFromJson>(data);
            Ballot ballot = new Ballot(deserializedBallot.BallotName);
            Contest contest;
            Candidate candidate;
            foreach (BallotFromJson.ContestHelper contestItem in deserializedBallot.Contests)
            {
                string contestData = File.ReadAllText($"CONTEST_{contestItem.ContestCode}.json");
                ContestFromJson deserializedContest = JsonSerializer.Deserialize<ContestFromJson>(contestData);
                contest = new Contest(contestItem.ContestCode, deserializedContest.ContestName, deserializedContest.MaxChoices);

                // look at the order of candidates in contestDeserialized.CandidateCodes
                foreach (string candidateCode in contestItem.CandidateCodes)
                {
                    // find the candidate with the code
                    ContestFromJson.CandidateHelper foundCandidate = deserializedContest.Candidates.First(x => x.CandidateCode == candidateCode);

                    // create and add the candidate to the contest
                    candidate = new Candidate(candidateCode, foundCandidate.CandidateName, foundCandidate.CandidateParty);
                    contest.AddCandidate(candidate);
                }

                // check if contest needs writeins
                if (deserializedContest.WriteIn) contest.AddBlankWriteIns();

                ballot.AddContest(contest);
            }
            return ballot;
        }

        static void Vote(Ballot ballot)
        {
            ballot.PrepForVoting();
            char userInput;
            bool done = false;
            do
            {
                ballot.DisplayCurrentCandidate();
                List<char> options = ballot.DisplayOptions();
                userInput = Console.ReadKey(true).KeyChar;

                // if user does not enter a valid option, set userInput to trigger default switch case
                if (!options.Contains(userInput)) userInput = '!';
                switch (userInput)
                {
                    case '0':
                        ballot.Output();
                        break;
                    case '2':
                        ballot.GoToPrevContest();
                        break;
                    case '4':
                        ballot.GoToPrevCandidate();
                        break;
                    case '5':
                        ballot.SelectCandidate();
                        break;
                    case '6':
                        ballot.GoToNextCandidate();
                        break;
                    case '8':
                        if (ballot.CurrentContest.IsLastContest)
                        {
                            done = true;
                            break;
                        }
                        ballot.GoToNextContest();
                        break;
                    default:
                        Console.WriteLine("Invalid key!");
                        break;
                }
            } while (!done);
            Console.WriteLine("Here is your final ballot:");
            ballot.Output();
            Console.WriteLine("\n...Program ended!");
        }
    }


}
