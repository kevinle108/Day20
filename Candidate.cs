using System;
using System.Collections.Generic;
using System.Text;

namespace Day20
{
    public class Candidate
    {

        public string Code { get; }
        public string Name { get; set; }
        public string Party { get; }
        public bool Selected { get; private set; }
        public bool IsWriteIn { get; }

        public Candidate(string code, string name, string party)
        {
            Code = code;
            Name = name;
            Party = party;
            Selected = false;
        }

        // constructor used for writeins
        public Candidate(string candType)
        {
            if (candType == "writein") {
                Code = "";
                Name = "";
                Party = "";
                Selected = false;
                IsWriteIn = true;
            }            
        }

        public string DisplayText()
        {
            string txt = "";
            if (IsWriteIn)
            {
                txt += $"Write-in: {Name}";
            }
            else
            {
                txt += $"{Name} ({Party})";
            }
            if (Selected)
            {
                txt += $" -- Selected";
            }
            return txt;
        }

        public void ToggleSelection()
        {
            if (Selected) Selected = false;
            else Selected = true;
        }
    }
}
