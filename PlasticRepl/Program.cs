using System;
using PlasticLang;

namespace PlasticRepl
{
    class Program
    {
        static void Main(string[] args)
        {
            PlasticContext context = Plastic.SetupCoreSymbols();
            var userContext = new PlasticContextImpl(context);
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("user> ");
                Console.ForegroundColor = ConsoleColor.Gray;
                var input = Console.ReadLine();
                try
                {
                    
                    var res = Plastic.Run(input, userContext);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("{0}", res);
                }
                catch (Exception x)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(x.Message);
                }
            }
        }
    }
}
