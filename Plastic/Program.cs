using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace PlasticLang
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var code = File.ReadAllText("fun.pla");
            var sw = Stopwatch.StartNew();
            Plastic.Run(code);
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            Console.ReadLine();
        }
    }
}