using System;
using System.Collections.Generic;
using System.Text;

namespace Day18
{
    class Ballot
    {
        private int _currentContestIndex;
        private Contest _currentContest;

        public string Name { get; }
        public List<Contest> Contests { get; }
        public Contest CurrentContest { get { return _currentContest; } }
        public int CurrentContestIndex 
        {
            get { return _currentContestIndex; }                                        
            private set
            {
                _currentContestIndex = value;
                _currentContest = Contests[_currentContestIndex];
            }
        }
        readonly Dictionary<char, string> AllOptions = new Dictionary<char, string>() 
        {
            {'0', "0: Display Ballot" },
            {'2', "2: Prev Contest" },
            {'4', "4: Prev Candidate" },
            {'5', "5: Select" },
            {'6', "6: Next Candidate" },
            {'8', "8: Next Contest" },
        };

        public Ballot(string name)
        {
            Name = name;
            Contests = new List<Contest>();
        }

        public void AddContest(Contest contest)
        {
            Contests.Add(contest);
        }

        public void Output()
        {
            Console.WriteLine(Name);
            for (int i = 0; i < Contests.Count; i++)
            {
                Console.WriteLine($"  Contest {i + 1} of {Contests.Count}: {Contests[i].Name} (Vote for {Contests[i].VoteFor})");
                for (int j = 0; j < Contests[i].Candidates.Count; j++)
                {
                    Console.WriteLine($"    {Contests[i].Candidates[j].DisplayText()}");
                }
            }
        }

        public void DisplayCurrentCandidate()
        {
            string txt = $"\n{CurrentContest.Name} - ";
            if (CurrentContest.CurrentCandidate.IsWriteIn) txt += $"(Write-in) {CurrentContest.CurrentCandidate.Name}";
            else txt += $"{CurrentContest.CurrentCandidate.Name} ({CurrentContest.CurrentCandidate.Party})";
            if (CurrentContest.CurrentCandidate.Selected) txt += " (Selected)";
            Console.WriteLine(txt);
        }       

        // sets the all the current contest and candidate indexes to begin voting 
        public void PrepForVoting()
        {
            CurrentContestIndex = 0;
            Contests[0].IsFirstContest = true;
            Contests[^1].IsLastContest = true;
            foreach (var contest in Contests)
            {
                contest.CurrentCandidateIndex = 0;
            }
        }
        
        public List<char> DisplayOptions()
        {
            List<char> options = new List<char>() { '0', '2', '4', '5', '6', '8' };
            var optionsSet = new Dictionary<char, string>(AllOptions);
            //0: Display Ballot
            //2: Prev Contest
            //4: Prev Candidate
            //5: Select
            //6: Next Candidate
            //8: Next Contest

            // remove any options not available to the situation
            if (CurrentContest.IsFirstContest)
            {
                options.RemoveAll(x => x == '2');                
            }
            if (CurrentContest.IsCurrentCandidateFirst)
            {
                options.RemoveAll(x => x == '4');
            }
            if (CurrentContest.IsCurrentCandidateLast)
            {
                options.RemoveAll(x => x == '6');
            }

            // modify any remaining options to the situation
            if (CurrentContest.IsLastContest)
            {
                optionsSet['8'] = "8: Done";
            }
            if (CurrentContest.CurrentCandidate.Selected)
            {
                optionsSet['5'] = "5: Deselect";
            }

            // print the options
            Console.Write("Press a key -- ");
            foreach (var item in options)
            {
                Console.Write(optionsSet.GetValueOrDefault(item) + "  ");
            }
            Console.WriteLine();

            // return the list of valid options
            return options;
        }
        public void GoToPrevContest()
        {
            if (!CurrentContest.IsFirstContest)
            {
                CurrentContestIndex--;
                CurrentContest.CurrentCandidateIndex = 0;
            }
        }

        public void GoToNextContest()
        {
            if (!CurrentContest.IsLastContest)
            {
                CurrentContestIndex++;
                CurrentContest.CurrentCandidateIndex = 0;
            }
        }
        public void GoToPrevCandidate()
        {
            if (!CurrentContest.IsCurrentCandidateFirst)
            {
                CurrentContest.CurrentCandidateIndex--;
            }
        }
        
        public void GoToNextCandidate()
        {
            if (!CurrentContest.IsCurrentCandidateLast)
            {
                CurrentContest.CurrentCandidateIndex++;
            }
        }



        public void SelectCandidate()
        {
            if (CurrentContest.NumOfVotes >= CurrentContest.VoteFor && !CurrentContest.CurrentCandidate.Selected)
            {
                Console.WriteLine("Overvote!");
                return;
            }
            CurrentContest.CurrentCandidate.ToggleSelection();
            if (CurrentContest.CurrentCandidate.Selected)
            {                
                if (CurrentContest.CurrentCandidate.IsWriteIn)
                {
                    Console.Write("Enter the writein name: ");
                    string writeinName = Console.ReadLine().Trim();
                    if (writeinName == "") 
                    {
                        // invalid name -> deselect the writein and return
                        CurrentContest.CurrentCandidate.ToggleSelection();
                        return;                    
                    }
                    CurrentContest.CurrentCandidate.Name = writeinName;
                }
                CurrentContest.NumOfVotes++;
            }
            if (!CurrentContest.CurrentCandidate.Selected)
            {
                if (CurrentContest.CurrentCandidate.IsWriteIn)
                {
                    CurrentContest.CurrentCandidate.Name = "";
                }
                CurrentContest.NumOfVotes--;
            }
        }        
    }
}
