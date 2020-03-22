using System;
using System.Collections.Generic;
using System.Text;
using DotNetGraph.Node;
using DotNetGraph.Edge;
using DotNetGraph;
using DotNetGraph.Extensions;

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
    class Nfa<T>
    {
        private List<List<NfaStateNode<T>>> NfaGraph;
        private DisjointSet Set { get; set; }
        public int BeginState { get; set; }
        public int SuccessState { get; set; }
        const int Nothing = -1;
        public Dfa ConvertToDfa()
        {
            return new Dfa();
        }
        public Nfa()
        {
            NfaGraph = new List<List<NfaStateNode<T>>>();
            Set = new DisjointSet();
        }

        public int AddNewState()
        {
            NfaGraph.Add(new List<NfaStateNode<T>>());
            Set.AddNew();
            return NfaGraph.Count - 1;
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
            return Set.Find(state);
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
        public string GenDotFile(string name)
        {
            DotNode node;
            DotGraph dotGraph = new DotGraph(name, true);

            List<int> allState;
            GetAllState(out allState);
            var s = "";
            foreach(var i in allState)
            {
                if(i == SuccessState)
                {
                    s = "Success";
                }
                else if (i == BeginState)
                {
                    s = "Begin";
                }
                else
                {
                    s = $"{i}";
                }
                node = new DotNode($"{i}")
                {
                    Shape = DotNodeShape.Circle,
                    Label = s
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

        public void GenPngFile(string name)
        {
            var i = GenDotFile(name);
            System.IO.File.WriteAllText($"./{name}.dot", i);
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory();
            startInfo.FileName = @"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe";
            startInfo.Arguments = $"-Tpng ./{name}.dot -o ./{name}.png";
            process.StartInfo = startInfo;
            process.Start();
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
