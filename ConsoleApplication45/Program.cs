using System;
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

            var res = PlasticParser.Statements.Parse(@"
let a = 1;
let b = 3;
let c = a + b;

print(c);
print(""c = "" + c);

");
            var context = new PlasticContext();
            PlasticInterop print = a =>
            {
                var v = a.FirstOrDefault();
                Console.WriteLine(v);
                return v;
            };

            context["print"] = print;
            res.Eval(context);
            Console.ReadLine();
        }
    }
}