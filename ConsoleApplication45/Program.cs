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
            var res = PlasticParser.Statements.Parse(@"
let a = 1;
let b = 3;
let c = a + b;
let printhello = (x) => print(x + a);
let a = a + 1;
print(""c = "" + c);
print(""a = "" + a);
printhello(""foo"");

(x => print(""lambda fun "" + x), x => print(""lambda fun2 "" + x))(""yay"");
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