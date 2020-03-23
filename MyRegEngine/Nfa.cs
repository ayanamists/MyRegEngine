using System;
using System.Collections.Generic;
using System.Text;
using DotNetGraph.Node;
using DotNetGraph.Edge;
using DotNetGraph;
using DotNetGraph.Extensions;
using System.Drawing;

namespace MyRegEngine
{
    class NfaStateNode<T>
    {
        public int State { get; set; }
        public T How { get; set; }
        public NfaStateNode(int state, T how)
        {
            State = state;
            How = how;
        }
    }
    class Nfa<T> : AutoMechine
    {
        private List<List<NfaStateNode<T>>> NfaGraph;
        private DisjointSet Set { get; set; }
        const int Nothing = -1;
        public Nfa()
        {
            NfaGraph = new List<List<NfaStateNode<T>>>();
            Set = new DisjointSet();
        }

        NfaStateInDfa GetEpsilonClosure(int state)
        {
            Queue<int> que = new Queue<int>();
            List<bool> mark = new List<bool>();
            var ret = new NfaStateInDfa();
            ret.AddState(state);
            for(var i = 0; i < NfaGraph.Count; ++i)
            {
                mark.Add(false);
            }
            que.Enqueue(state);

            while(que.Count != 0)
            {
                var now = que.Dequeue();
                mark[now] = true;
                foreach(var i in NfaGraph[now])
                {
                    if(i.How.Equals(new CharWithEmpty()) && mark[i.State] == false)
                    {
                        ret.AddState(i.State);
                        que.Enqueue(i.State);
                    }
                }
            }
            return ret;
        }

        NfaStateInDfa GetEpsilonClosure(NfaStateInDfa stateUnion)
        {
            Queue<int> que = new Queue<int>();
            List<bool> mark = new List<bool>();
            var ret = stateUnion;
            for (var i = 0; i < NfaGraph.Count; ++i)
            {
                mark.Add(false);
            }
            foreach(var i in stateUnion)
            {
                que.Enqueue(i);
            }

            while (que.Count != 0)
            {
                var now = que.Dequeue();
                mark[now] = true;
                foreach (var i in NfaGraph[now])
                {
                    if (i.How.Equals(new CharWithEmpty()) && mark[i.State] == false)
                    {
                        ret.AddState(i.State);
                        que.Enqueue(i.State);
                    }
                }
            }
            return ret;
        }
        public void FillDfa(ref Dfa<T> dfa)
        {
            Queue<int> que = new Queue<int>();

            NfaStateInDfa sbegin = GetEpsilonClosure(BeginState);
            var begin = dfa.AddNewState(sbegin);
            que.Enqueue(begin);
            dfa.BeginState = begin;
            dfa.SuccessState = SuccessState;
            while(que.Count != 0)
            {
                var now = que.Dequeue();
                var NfaStateOfNow = dfa.GetNfaState(now);

                Dictionary<T, NfaStateInDfa> target = new Dictionary<T, NfaStateInDfa>();
                foreach(var i in NfaStateOfNow)
                {
                    foreach(var j in NfaGraph[i])
                    {
                        if(j.How.Equals(new CharWithEmpty())) { continue; }
                        if (target.ContainsKey(j.How))
                        {
                            target[j.How].AddState(GetReal(j.State));
                        }
                        else
                        {
                            target[j.How] = new NfaStateInDfa();
                            target[j.How].AddState(GetReal(j.State));
                        }
                    }
                }

                foreach(var i in target)
                {
                    var newS = 0;
                    var thisUoion = GetEpsilonClosure(i.Value);
                    if (!dfa.ContainNfaState(thisUoion))
                    {
                        newS = dfa.AddNewState(thisUoion);
                        que.Enqueue(newS);
                    }
                    else
                    {
                        newS = dfa.GetStateByNfaState(thisUoion);
                    }
                     dfa.SetStateTransition(now, newS, i.Key);
                }
            }
        }
        public int AddNewState()
        {
            NfaGraph.Add(new List<NfaStateNode<T>>());
            Set.AddNew();
            return NfaGraph.Count - 1;
        }

        private List<int> GetAllAdjustState(int s)
        {
            var realTarget = GetReal(s);
            var ret = new List<int>();
            foreach(var i in NfaGraph[realTarget])
            {
                ret.Add(GetReal(i.State));     
            }
            return ret;
        }
        public int AddSuccessState()
        {
            var i = AddNewState();
            SuccessState = i;
            return i;
        }
        public int Merge(int x, int y)
        {
            return Set.Merge(x, y);
        }
        public void SetStateTransition(int stateBefore, int stateAfter, T how)
        {
            NfaGraph[stateBefore].Add(new NfaStateNode<T>(stateAfter, how));
        }
        private int GetReal(int state)
        {
            return state;
        }
        private void GetAllState(out List<int> allState)
        {
            List<bool> Exist = new List<bool>(NfaGraph.Count);
            for(int i = 0;i < NfaGraph.Count; ++i) { Exist.Add(false); }
            allState = new List<int>();
            for(var i = 0; i < NfaGraph.Count; ++i)
            {
                var j = GetReal(i);
                if (Exist[j]) { continue; }
                else
                {
                    Exist[j] = true;
                    allState.Add(j);
                }
            }
        }
        public override string GenDotFile(string name)
        {
            DotNode node;
            DotGraph dotGraph = new DotGraph(name, true);

            List<int> allState;
            GetAllState(out allState);
            var s = "";
            var color = Color.White;
            foreach(var i in allState)
            {
                color = (i == BeginState) ? Color.Blue : Color.Black;
                s = GetStateName(i, (x)=> { return $"{x}"; }); 
                node = new DotNode($"{i}")
                {
                    Shape = DotNodeShape.Circle,
                    Label = s,
                    Color = color
                };
                dotGraph.Elements.Add(node);
            }

            for(int i = 0; i < NfaGraph.Count; ++i)
            {
                var RealI = GetReal(i);
                foreach(var j in NfaGraph[i])
                {
                    var edge = new DotEdge($"{RealI}", $"{GetReal(j.State)}")
                    {
                        Label = $"{j.How}"
                    };
                    dotGraph.Elements.Add(edge);
                }
            }
            var ret = dotGraph.Compile();
            return ret; 
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
