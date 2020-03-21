using System;
using System.IO;

namespace MyRegEngine
{
    class Program
    {
        static int count = 0;
        static void RunTestNfa(string s)
        {
            var regex = new Regex(s);
            regex.GenNfaPng($"TestCase{count++}");
        }
        static void Main(string[] args)
        {
            RunTestNfa("a");
            RunTestNfa("ab");
            RunTestNfa("a*");
            RunTestNfa("a|b");
            RunTestNfa("a+");
            RunTestNfa("a?");
            RunTestNfa("a*?");
            RunTestNfa(".*abcd.*");
            RunTestNfa("(aaa|c)*a+?b");
        }
    }
}
