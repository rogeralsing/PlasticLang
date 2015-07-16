using System;
using System.Collections;
using System.Linq;
using PlasticLangLabb1.Ast;
using Sprache;

namespace PlasticLangLabb1
{
    public static class Plastic
    {
        public static void Run(string code)
        {
            var context = SetupCoreSymbols();
            BootstrapLib(context);

            var userContext = context.ChildContext();
            var res = PlasticParser.Statements.Parse(code);
            res.Eval(userContext);
        }

        private static void BootstrapLib(PlasticContext context)
        {
            var lib = @"
for := func (init.ref , guard.ref, step.ref, body.ref)
{
    init()
    while(guard())
    {
        body()
        step()
    }
}

repeat := func (times, body.ref)
{
    while(times >= 0)
    {
        body()
        times--
    }
}

LinkedList = class 
{
    Node = class (value) { next = null; }

    head = null;
    tail = null;
    add = func (value)
    {
        node = Node(value);
        if (head == null)
        {         
            head = node;
            tail = node;
        }
        else
        {
            tail.next =  node;
            tail = node;  
        }        
    }

    each = func (lambda)
    {
        current = head;
        while(current != null)
        {
            lambda(current.value);
            current = current.next;
        }
    }
}

Stack = class
{
    Node = class (value,prev) { next = null; }

    head = null;
    tail = null;
    push = func (value)
    {
        node = Node(value,tail);
        if (head == null)
        {         
            head = node;
            tail = node;
        }
        else
        {
            tail.next =  node;
            tail = node;  
        }        
    }

    each = func (lambda)
    {
        current = tail;
        while(current != null)
        {
            lambda(current.value);
            current = current.prev;
        }
    }

    peek = func()
    {
        tail.value;
    }

    pop = func()
    {
        res = tail.value;
        tail = tail.prev;
        if (tail != null)
        {
            tail.next = null;
        }
        else
        {
            head = null;
        }
        res
    }
}
";


            var libCode = PlasticParser.Statements.Parse(lib);
            libCode.Eval(context);
        }

        private static PlasticContext SetupCoreSymbols()
        {
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

            PlasticMacro func = (c, a) =>
            {
                var Args = a.Take(a.Length - 1).Select(arg =>
                {
                    var identifier = arg as Identifier;
                    if (identifier != null)
                    {
                        return new Argument(identifier.Name, ArgumentType.Value);
                    }
                    var dot = arg as BinaryExpression;
                    if (dot != null)
                    {
                        var id = dot.Left as Identifier;
                        var type = dot.Right as Identifier;
                        ArgumentType argType = ArgumentType.Value;
                        if (type.Name == "ref")
                            argType = ArgumentType.Expression;

                        return new Argument(id.Name, argType);
                    }
                    throw new NotSupportedException();
                }).ToArray();
                var Body = a.Last();

                PlasticMacro op = null;
                op = (callingContext, args) =>
                {
                    //full application
                    if (args.Length == Args.Length)
                    {
                        //create context for this invocation
                        var ctx = callingContext.ChildContext();
                        for (var i = 0; i < args.Length; i++)
                        {
                            var arg = Args[i];
                            if (arg.Type == ArgumentType.Expression)
                            {
                                //copy args from caller to this context
                                ctx.Declare(arg.Name, args[i]);
                            }
                            else if (arg.Type == ArgumentType.Value)
                            {
                                var value = args[i].Eval(callingContext);
                                ctx.Declare(arg.Name, value);
                            }
                        }

                        var m = Body.Eval(ctx);
                        return m;
                    }
                    //partial application
                    var partialArgs = args.ToArray();

                    PlasticMacro partial = (ctx,pargs) => op(ctx,partialArgs.Union(pargs).ToArray());

                    return partial;
                };
                return op;
            };

            
            PlasticMacro @class = (c, a) =>
            {
                var body = a.Last();
                PlasticFunction f = args =>
                {
                    var thisContext = c.ChildContext();

                    for (var i = 0; i < a.Length - 1; i++)
                    {
                        var argName = a[i] as Identifier;
                        thisContext.Declare(argName.Name, args[i]);
                    }

                    var self = new PlasticObject(thisContext);
                    thisContext.Declare("this", self);
                    body.Eval(thisContext);

                    return self;
                };
                return f;
            };


            context.Declare("print", print);
            context.Declare("while", @while);
            context.Declare("each", each);
            context.Declare("if", @if);
            context.Declare("elif", @elif);
            context.Declare("else", @else);
            context.Declare("true", true);
            context.Declare("false", false);
            context.Declare("null", null);
            context.Declare("exit", exit);
            context.Declare("func", func);
            context.Declare("class", @class);
            return context;
        }
    }
}