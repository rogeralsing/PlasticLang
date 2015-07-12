using System.Linq;
using PlasticLangLabb1.Ast;
using Sprache;

namespace PlasticLangLabb1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var x = PlasticParser.Expression.Parse("(axy) => { abc; }");

            var res = PlasticParser.Body.Parse(@"
{ 
    (123) + 555; 

    abc;

    123;

    (abc + 123 * xyz def) - 77;

    (axy) => { abc; };
}");
        }
    }
}