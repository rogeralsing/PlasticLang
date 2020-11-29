using System;
using System.Diagnostics;
using System.IO;
using PlasticLang.Contexts;

namespace PlasticLang
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Plastic.Run("");
            var code = File.ReadAllText("fun.pla");
            Plastic.Run(code);
            Benchmark(code);

        }

        private static void Benchmark(string code)
        {
            var sw = Stopwatch.StartNew();
            Plastic.Run(code);
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }
    }
}