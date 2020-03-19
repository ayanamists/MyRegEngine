using System;
using System.Collections.Generic;
using System.Text;

namespace MyRegEngine
{
    public class Regex
    {
        public string RegexStr {get;set;}
        private Dfa dfa;
        public Regex() { }
        public Regex(string regexStr)
        {
            RegexStr = regexStr;
        }
        public bool match(string target)
        {
            return false;
        }
        void GenDfa()
        {

        }
    }
}
