using System;

namespace MyRegEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            var regex = new Regex("(.*aa|cc)bb.*?+9*");
            System.Console.WriteLine(regex.RegexStr);
        }
    }
}
