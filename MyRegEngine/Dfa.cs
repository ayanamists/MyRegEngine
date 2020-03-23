using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using DotNetGraph;
using DotNetGraph.Edge;
using DotNetGraph.Node;
using DotNetGraph.Extensions;
using System.Drawing;

namespace MyRegEngine
{
    internal class NfaStateInDfa
    {
        private SortedSet<int> set = new SortedSet<int>();
        public NfaStateInDfa()
        {
        }
        public void AddState(int s)
        {
            if (!set.Contains(s))
            {
                set.Add(s);
            }
        }
        public override bool Equals(object obj)
        {
            var other = (NfaStateInDfa)obj;
            return set.Overlaps(other.set);
        }
        public bool Contains(int state)
        {
            return set.Contains(state);
        }
        public override int GetHashCode()
        {
            int hash = 0;
            int count = 1;
            foreach(var i in set)
            {
                hash ^= i.GetHashCode() + count * 0x1af3 + 47832;
            }
            return hash;
        }
        public IEnumerator<int> GetEnumerator()
        {
            return set.GetEnumerator();
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var i in set)
            {
                stringBuilder.Append(i);
                stringBuilder.Append(" ");
            }
            return stringBuilder.ToString();
        }
    }
    class Dfa<T> : AutoMechine
    {
        private List<List<int>> DfaGraph;
        private List<bool> Mark = new List<bool>();
        private Dictionary<T, int> AlphaBetMapping = new Dictionary<T, int>();
        private List<T> ReverseAlphaBetMapping;

        private Dictionary<NfaStateInDfa, int> StateMapping = new Dictionary<NfaStateInDfa, int>();
        private List<NfaStateInDfa> ReverseStateMapping = new List<NfaStateInDfa>();
        const int Nothing = -1;
        const int SearchCache = 0;

        public bool ContainNfaState(in NfaStateInDfa state)
        {
            return StateMapping.ContainsKey(state);
        }

        public int GetStateByNfaState(in NfaStateInDfa state)
        {
            var ret = 0;
            StateMapping.TryGetValue(state, out ret);
            return ret;
        }
        public Dfa(in List<T> Chars)
        {
            DfaGraph = new List<List<int>>();
            var count = 0;
            foreach(var i in Chars)
            {
                AlphaBetMapping.Add(i, count++);
            }
            ReverseAlphaBetMapping = Chars;
            ReverseStateMapping = new List<NfaStateInDfa>();
        }
        public int AddNewState(in NfaStateInDfa vs)
        {
            var list = new List<int>();
            for(int i = 0; i < AlphaBetMapping.Count; ++i)
            {
                list.Add(Nothing);
            }
            DfaGraph.Add(list);

            if (vs.Contains(SuccessState))
            {
                Mark.Add(true);
            }
            else
            {
                Mark.Add(false);
            }
            var newState = DfaGraph.Count - 1;
            StateMapping.Add(vs, newState);
            ReverseStateMapping.Add(vs);
            System.Console.WriteLine($"Add State:{newState} -> {GetDfaStateName(newState)} ");
            return newState;
        }

        public void SetStateTransition(int state1, int state2, T how)
        {
            var index = AlphaBetMapping[how];
            DfaGraph[state1][index] = state2;
            System.Console.WriteLine($"Add State Transition: {state1}, {state2}, {how}");
        }

        public NfaStateInDfa GetNfaState(int state)
        {
            return ReverseStateMapping[state];
        }
        public string GetDfaStateName(int s) {
            StringBuilder @string = new StringBuilder();
            foreach(var i in ReverseStateMapping[s])
            {
                var str = GetStateName(i, (x) => { return $"{x}"; });
                @string.Append(str);
                @string.Append("|");
            }
            return @string.ToString();
        }
        public override string GenDotFile(string name)
        {
            var graph = new DotGraph(name, true);
            string s = "";
            var color = Color.White;
            for (int i = 0; i < DfaGraph.Count; ++i)
            {
                s = GetDfaStateName(i);
                color = (i == BeginState) ? Color.Blue : Color.Black;
                var node = new DotNode($"{i}")
                {
                    Shape = DotNodeShape.Circle,
                    Label = s,
                    Color = color
                };
                graph.Elements.Add(node);
            }

            for(int i = 0; i < DfaGraph.Count; ++i)
            {
                for(int j = 0; j < DfaGraph[i].Count; ++j)
                {
                    if (DfaGraph[i][j].Equals(Nothing)) { continue; }
                    else
                    {
                        var edge = new DotEdge($"{i}", $"{DfaGraph[i][j]}")
                        {
                            Label = $"{ReverseAlphaBetMapping[j]}"
                        };
                        graph.Elements.Add(edge);
                    }
                }
            }
            
            return graph.Compile();
        }
    }
}