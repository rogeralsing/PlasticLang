using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlasticLangLabb1.Ast;
using Sprache;

namespace PlasticLangLabb1
{
    public static class Plastic
    {
        public static void Run(string code)
        {
            var lib = @"
for := macro (init, guard, step, body)
{
    init()
    while(guard())
    {
        body()
        step()
    }
}

repeat := macro (times, body)
{
    i := times();
    while(i >= 0)
    {
        body()
        i--
    }
}
";
            var exit = new object();
            var context = new PlasticContext();
            PlasticFunction print = a =>
            {
                var v = a.FirstOrDefault();
                Console.WriteLine(v);
                return v;
            };

            PlasticMacro @while = (c, a) =>
            {
                var result = exit;
                var cond = a[0];
                var body = a[1];

                while ((bool) cond.Eval(c))
                {
                    result = body.Eval(c);
                }

                return result;
            };

            PlasticMacro @if = (c, a) =>
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

                if ((bool) cond.Eval(c))
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
                    c.Declare(v.Name, element);
                    body.Eval(c);
                }
                return result;
            };

            PlasticMacro macro = (c, a) =>
            {
                var Args = a.Take(a.Length - 1).Cast<Identifier>();
                var Body = a.Last();

                PlasticMacro op = (callingContext, args) =>
                {
                    //create context for this invocation
                    var ctx = callingContext.ChildContext();
                    int i = 0;
                    foreach (var arg in Args)
                    {
                        //copy args from caller to this context
                        ctx[arg.Name] = args[i];
                        i++;
                    }

                    var m = Body.Eval(callingContext);
                    return m;
                };
                return op;
            };

            PlasticMacro func = (c, a) =>
            {
                var Args = a.Take(a.Length - 1).Cast<Identifier>().ToArray();
                var Body = a.Last();

                PlasticFunction op = null;
                op = args =>
                {
                    //full application
                    if (args.Length == Args.Length)
                    {
                        //create context for this invocation
                        var ctx = c.ChildContext();
                        for (int i = 0; i < args.Length; i++)
                        {
                            var arg = Args[i];
                            //copy args from caller to this context
                            ctx[arg.Name] = args[i];
                        }

                        var x = Body.Eval(ctx);
                        return x;
                    }
                    else
                    {
                        //partial application
                        object[] partialArgs = args.ToArray();

                        PlasticFunction partial = pargs => 
                            op(partialArgs.Union(pargs).ToArray());
                        
                        return partial;
                    }
                };
                return op;
            };

            PlasticMacro @class = (c, a) =>
            {
                var body = a.Last();
                PlasticFunction f = (args) =>
                {
                    var thisContext = c.ChildContext();

                    for (int i = 0; i < a.Length - 1;i++)
                    {
                        var argName = a[i] as Identifier;
                        thisContext.Declare(argName.Name,args[i]);
                    }

                    var self = new PlasticObject(thisContext);
                    thisContext.Declare("this", self);
                    body.Eval(thisContext);

                    return self;
                };
                return f;
            };


            context["print"] = print;
            context["while"] = @while;
            context["each"] = each;
            context["if"] = @if;
            context["elif"] = @elif;
            context["else"] = @else;
            context["true"] = true;
            context["false"] = false;
            context["null"] = null;
            context["exit"] = exit;
            context["macro"] = macro;
            context["func"] = func;
            context["class"] = @class;

            var libCode = PlasticParser.Statements.Parse(lib);
            libCode.Eval(context);

            var res = PlasticParser.Statements.Parse(code);
            res.Eval(context);
        }
    }
}