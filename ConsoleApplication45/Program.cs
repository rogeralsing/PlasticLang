using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using PlasticLangLabb1.Ast;
using Sprache;

namespace PlasticLangLabb1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var res = PlasticParser.Statements.Parse(@"
let a = 1
let b = 3
let c = a + b
a = a + 1
print('c = ' + c)
print('a = ' + a)

let closureprint = x => print(x + a)
closureprint('foo')

if (a == 1)
{
    print ('inside if')
}
elif (a == 3)
{
    print ('inside elif')
}
else
{
    print ('inside else')
}

while (a < 20)
{
     print ('daisy me rollin`')
     a = a+1
}

(x => print('lambda fun ' + x), x => print('lambda fun2 ' + x))('yay')
");
            object exit = new object();            
            var context = new PlasticContext();
            PlasticFunction print = a =>
            {
                var v = a.FirstOrDefault();
                Console.WriteLine(v);
                return v;
            };

            PlasticMacro @while = (c,a) =>
            {
                object result = exit;
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
                var cond = a[0];
                var body = a[1];

                if ((bool) cond.Eval(c))
                {
                    return body.Eval(c);
                }

                return exit;
            };

            PlasticMacro @elif = (c, a) =>
            {
                var last = c["last"];
                if (last != exit)
                    return last;

                var cond = a[0];
                var body = a[1];

                if ((bool)cond.Eval(c))
                {
                    return body.Eval(c);
                }

                return exit;
            };

            PlasticMacro @else = (c, a) =>
            {
                var last = c["last"];
                if (last != exit)
                    return last;

                var body = a[0];

                return body.Eval(c);
            };

            context["print"] = print;
            context["while"] = @while;
            context["if"] = @if;
            context["elif"] = @elif;
            context["else"] = @else;
            context["true"] = true;
            context["false"] = false;
            context["false"] = false;
            context["exit"] = exit;

            res.Eval(context);
            Console.ReadLine();
        }
    }
}