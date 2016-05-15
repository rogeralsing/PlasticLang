using System;
using PlasticLang;

namespace PlasticRepl
{
    class Program
    {
        static void Main(string[] args)
        {
            PlasticContext context = Plastic.SetupCoreSymbols().Result;
            
            var userContext = new PlasticContextImpl(context);
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("user> ");
                Console.ForegroundColor = ConsoleColor.Gray;
                var input = Console.ReadLine();
                try
                {

                    var res = Plastic.Run(input, userContext).Result;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("{0}", res);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("{0}", res != null ? res.GetType().Name:"null");
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
