using System;
using System.IO;

namespace PlasticLang
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var code = File.ReadAllText("sample.pla");
            Plastic.Run(code);
            Console.ReadLine();
        }
    }
}