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
print('c = ' + c);
print('a = ' + a);
printhello('foo');

if (a == 2)
{
    print ('inside if');
};

(x => print('lambda fun ' + x), x => print('lambda fun2 ' + x))('yay');
");
            var context = new PlasticContext();
            PlasticFunction print = a =>
            {
                var v = a.FirstOrDefault();
                Console.WriteLine(v);
                return v;
            };

            PlasticMacro @while = (c,a) =>
            {
                object result = null;
                var cond = a[0];
                var body = a[1];

                while ((bool)cond.Eval(c))
                {
                    result = body.Eval(c);
                }

                return result;
            };

            PlasticMacro @if = (c,a) =>
            {
                object result = null;
                var cond = a[0];
                var body = a[1];

                if ((bool) cond.Eval(c))
                {
                    result = body.Eval(c);
                    return result;
                }

                return result;
            };

            context["print"] = print;
            context["while"] = @while;
            context["if"] = @if;
            res.Eval(context);
            Console.ReadLine();
        }
    }
}