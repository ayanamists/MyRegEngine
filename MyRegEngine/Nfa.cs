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
        }

        public int AddNewState()
        {
            NfaGraph.Add(new List<NfaStateNode<T>>());
            return NfaGraph.Count - 1;
        }
        public int AddSuccessState()
        {
            var i = AddNewState();
            SuccessState = i;
            return i;
        }
        public void SetStateTransition(int stateBefore, int stateAfter, T how)
        {
            NfaGraph[stateBefore].Add(new NfaStateNode<T>(stateAfter, how));
        }
        public string GenDotFile(string name)
        {
            DotNode node;
            DotGraph dotGraph = new DotGraph(name, true);
            for(int i = 0; i < NfaGraph.Count - 1; ++i)
            {
                node = new DotNode($"{i}") {
                    Shape = DotNodeShape.Circle,
                    Label = $"{i}"
                };
                dotGraph.Elements.Add(node);
            }
            node = new DotNode("success")
            {
                Shape = DotNodeShape.Circle,
                Label = "success!"
            };
            dotGraph.Elements.Add(node); 

            for(int i = 0; i < NfaGraph.Count; ++i)
            {
                foreach(var j in NfaGraph[i])
                {
                    var s = "";
                    if(j.State != SuccessState)
                    {
                        s = $"{j.State}";
                    }
                    else
                    {
                        s = "success";
                    }
                    var edge = new DotEdge($"{i}", s)
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
