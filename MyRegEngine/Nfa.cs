using System;
using System.Collections.Generic;
using System.Text;

namespace MyRegEngine
{
    class Nfa<T>
    {
        private List<List<int>> NfaGraph;
        private Dictionary<T, int> NfaAlphabet;
        private int alphabetCount = 0;
        const int Nothing = -1;
        public Dfa ConvertToDfa()
        {
            return new Dfa();
        }
        public Nfa()
        {
            NfaGraph = new List<List<int>>();
        }

        public void AddNewState()
        {
            NfaGraph.Add(new List<int>(NfaAlphabet.Count));
        }
        public void SetAlphaBet(in List<T> alphabet)
        {
            foreach (var i in alphabet)
            {
                NfaAlphabet.Add(i, alphabetCount);
                alphabetCount++;
            }
        }
        public Nfa(int sizeOfState, in List<T> alphabet)
        {
            NfaGraph = new List<List<int>>(sizeOfState);
            SetAlphaBet(in alphabet);
            for(int i = 0;i < sizeOfState; ++i)
            {
                NfaGraph[i] = new List<int>(sizeOfState);
                for(int j = 0; i < NfaAlphabet.Count; ++j)
                {
                    NfaGraph[i][j] = Nothing;
                }
            }
        }
        private int GetInnerPosition(T what)
        {
            var innerPosition = 0;
            NfaAlphabet.TryGetValue(what, out innerPosition);
            return innerPosition;
        }
        public void SetStateTransition(int stateBefore, int stateAfter, T how)
        {
            var innerPosition = GetInnerPosition(how);
            NfaGraph[stateBefore][innerPosition] = stateAfter;
        }

        public override string ToString()
        {
            System.Text.StringBuilder stringBuilder = new StringBuilder();
            foreach(var i in NfaGraph)
            {
                foreach(var j in i)
                {
                    stringBuilder.Append(j);
                    stringBuilder.Append(" ");
                }
                stringBuilder.Append("\n");
            }
            return stringBuilder.ToString();
        }
    }
}
