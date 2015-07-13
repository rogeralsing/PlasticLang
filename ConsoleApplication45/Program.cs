using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using PlasticLangLabb1.Ast;
using Sprache;
using System.Collections;

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

let arr  = ['hello','this','is','an','array']

let closureprint = x => print(x + a)
closureprint('foo')

let for = (init, guard, step, body) #>
          {
                init()
                while(guard())
                {
                    body()
                    step()
                }
          }

each(element; arr)
{
    print(element)
}

for (a = 0; a < 10; a = a +1) 
{
    print (a);
}

let repeat = (times, body) #> {
                let i = times();
                while(i >= 0)
                {
                    body()
                    i--
                }
             };

repeat(3)
{
    print('repeat..')
}

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
     a++
}

(x => print('lambda fun ' + x), x => print('lambda fun2 ' + x))('yay')

if (true, print('hello'));
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

            PlasticMacro each = (c, a) =>
            {
                var v = a[0] as Identifier;
                var body = a[2];

                var enumerable = a[1].Eval(c) as IEnumerable;
                if (enumerable == null)
                    return exit;

                object result = null;
                foreach (var element in enumerable)
                {
                    c.Declare(v.Name,element);
                    body.Eval(c);
                }
                return result;
            };

            context["print"] = print;
            context["while"] = @while;
            context["each"] = each;
            context["if"] = @if;
            context["elif"] = @elif;
            context["else"] = @else;
            context["true"] = true;
            context["false"] = false;
            context["exit"] = exit;

            res.Eval(context);
            Console.ReadLine();
        }
    }
}


/*

let arr = [0, 'hello']
let obj = [a:0, b:'hello']
obj['a'] 

match (obj; [a[c],b]; print (c))

*/