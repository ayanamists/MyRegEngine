using System;
using System.Collections.Generic;
using System.Text;

namespace MyRegEngine
{
    abstract class AutoMechine
    {
        public abstract string GenDotFile(string name);
        public int BeginState { get; set; }
        public int SuccessState { get; set; }

        public string GetStateName(int s, Func<int, string> func)
        {
            if( s== SuccessState)
            {
                return "Success"; 
            }
            else
            {
                return $"{func(s)}";
            }
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
    }
}
