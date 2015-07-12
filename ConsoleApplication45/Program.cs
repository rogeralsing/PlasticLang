using System.Linq;
using PlasticLangLabb1.Ast;
using Sprache;

namespace PlasticLangLabb1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
           

            var res = PlasticParser.Expression.Parse(" (123) + 555");
        }
    }
}