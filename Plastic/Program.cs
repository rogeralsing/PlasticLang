using System;
using System.IO;
using System.Threading.Tasks;

namespace PlasticLang
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var code = File.ReadAllText("sample.pla");
            await Plastic.Run(code);
            Console.ReadLine();
        }
    }
}