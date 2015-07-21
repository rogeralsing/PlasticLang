using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                Console.Write("<< ");
                var input = Console.ReadLine();
                var res = Plastic.Run(input, userContext);
                Console.WriteLine(">> {0}",res);
            }
        }
    }
}
