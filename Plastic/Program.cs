using System;
using System.Diagnostics;
using System.IO;

namespace PlasticLang
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Plastic.Run("");
            var code = File.ReadAllText("fun.pla");
            var sw = Stopwatch.StartNew();
            Plastic.Run(code);
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }
    }
}