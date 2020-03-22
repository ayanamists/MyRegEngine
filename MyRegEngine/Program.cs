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
            regex.GenNfaPng($"TestCase{count}");
            regex.GenDfaPng($"TestCaseD{count}");
            count++;
        }
        static void Main(string[] args)
        {
            RunTestNfa("a");
            RunTestNfa("ab");
            RunTestNfa("a*");
            RunTestNfa("(a|b)*abcd");
            RunTestNfa("(ab+)|a*");
            RunTestNfa("a|b");
            RunTestNfa("a+");
            RunTestNfa("a?");
            RunTestNfa("a*?");
            RunTestNfa("a*abcde*");
            RunTestNfa("(aaa|ac)*a+?b");
            RunTestNfa("(a*b)|((adc)+(a|b)d?)");
        }
    }
}
