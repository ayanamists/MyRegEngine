using System;
using System.Collections.Generic;
using System.Text;

namespace MyRegEngine
{
    public class Regex
    {
        public string RegexStr {get;set;}
        static List<char> NormalAlphabet = 
            new List<char> 
            { 
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 
                'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 
                't', 'u', 'v', 'w', 'x', 'y', 'z', 
                '0','1', '2', '3', '4', '5', '6','7', '8', '9',
                '\n'
            };
        static HashSet<char> NormalMap = new HashSet<char>(NormalAlphabet);
        static List<char> OperatorAlphabet = 
            new List<char>
            {
                '*', '?', '+',
                '@', // '@' means concentration
                '|', '(', ')', '\\'
            };
        static HashSet<char> OperatorMap = new HashSet<char>(OperatorAlphabet);
        static List<char> SpecialAlphabet =
            new List<char>
            {
                '.'
            };
        static HashSet<char> SpecialMap = new HashSet<char>(SpecialAlphabet);
        private Dfa dfa;
        public Regex() { }
        public Regex(string regexStr)
        {
            RegexStr = regexStr;
            AddConcertration();
        }
        void AddConcertration()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for(int i = 0; i < RegexStr.Length; ++i)
            {
                if(RegexStr[i] == '@')
                {
                    stringBuilder.Append("\\@");
                }
                else
                {
                    stringBuilder.Append(RegexStr[i]);
                }
            }
            RegexStr = stringBuilder.ToString();
            stringBuilder.Clear();
            bool ifAdd = false;
            int next = 0;
            for(int i = 0; i < RegexStr.Length - 1;)
            {
                if(RegexStr[i] == '\\')
                {
                    next = i + 2;
                }
                else
                {
                    next = i + 1;
                }

                if(RegexStr[i] == '|' || RegexStr[i] == '(')
                {
                    ifAdd = false;
                }
                else if(NormalMap.Contains(RegexStr[next]) 
                    || SpecialMap.Contains(RegexStr[next]))
                {
                    ifAdd = true;
                }
                else if(OperatorMap.Contains(RegexStr[next]))
                {
                    if(RegexStr[next] == '\\' || RegexStr[next] == '(')
                    {
                        ifAdd = true;
                    }
                    else
                    {
                        ifAdd = false;
                    }
                }
                else
                {
                    throw new System.Exception("Regex Expression include unacceptable character");
                }

                for(int j = i; j < next; ++j)
                {
                    stringBuilder.Append(RegexStr[j]);
                }
                if (ifAdd)
                {
                    stringBuilder.Append('@');
                }
                i = next;
            }
            stringBuilder.Append(RegexStr[RegexStr.Length - 1]);
            RegexStr = stringBuilder.ToString();
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
