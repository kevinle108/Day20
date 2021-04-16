using System;
using System.Collections.Generic;
using System.Text;

namespace Day20
{
    class Contest
    {
        private int _currentCandidateIndex;
        private Candidate _currentCandidate;      
        public string Code { get; }        
        public string Name { get; }
        public int VoteFor { get; }
        public List<Candidate> Candidates { get; }
        public Candidate CurrentCandidate { get { return _currentCandidate; } }
        public int CurrentCandidateIndex 
        {
            get { return _currentCandidateIndex; }
            set 
            {
                _currentCandidateIndex = value;
                _currentCandidate = Candidates[_currentCandidateIndex];
            }        
        }
        public int NumOfVotes { get; set; }
        public bool IsFirstContest { get; set; }
        public bool IsLastContest { get; set; }
        public bool IsCurrentCandidateFirst { get { return _currentCandidateIndex == 0; } }
        public bool IsCurrentCandidateLast { get { return _currentCandidateIndex == Candidates.Count -1; } }


        public Contest(string code, string name, int voteFor)
        {
            Code = code;
            Name = name;
            VoteFor = voteFor;
            Candidates = new List<Candidate>();
            IsFirstContest = false;
            IsLastContest = false;
            NumOfVotes = 0;
        }

        public void AddCandidate(Candidate cand)
        {
            Candidates.Add(cand);                 
        }
        
        public void AddBlankWriteIns()
        {
            for (int i = 0; i < VoteFor; i++)
            {
                Candidates.Add(new Candidate("writein"));
            }            
        }

        public void Print()
        {
            Console.WriteLine(Name);
            foreach (Candidate cand in Candidates)
            {
                Console.WriteLine(cand.DisplayText());                
            }
        }

    }
}
