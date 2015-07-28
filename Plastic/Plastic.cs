using System;
using System.Collections;
using System.Linq;
using PlasticLang.Ast;
using Sprache;

namespace PlasticLang
{
    public static class Plastic
    {
        public static void Run(string code)
        {
            var context = SetupCoreSymbols();
            var userContext = new PlasticContextImpl(context);
            Run(code, userContext);
        }

        public static object Run(string code,PlasticContext context)
        {
            var res = PlasticParser.Statements.Parse(code);
            return res.Eval(context);
        }

        public static void BootstrapLib(PlasticContext context)
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

    each = func ( lambda)
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


switch :=  func(exp, body.ref)
{
    matched = false;
    case := func (value, caseBody.ref)
    {   
        if (exp == value)
        {
            caseBody();
            matched = true;
        }
    }
    default := func (defaultBody.ref)
    {
        if (matched == false)
        {
            defaultBody();
        }
    }
    body();
}

";


            var libCode = PlasticParser.Statements.Parse(lib);
            libCode.Eval(context);
        }

        public static PlasticContext SetupCoreSymbols()
        {
            var exit = new object();
            var context = new PlasticContextImpl();
            PlasticMacro print = (c, a) =>
            {
                var obj = a.First().Eval(c);
                var args = a.Skip(1).Select(o => o.Eval(c)).ToArray();
                if (args.Any())
                {
                    Console.WriteLine(obj.ToString(), args);
                }
                else
                {
                    Console.WriteLine(obj);
                }
                return obj;
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
                    var res = body.Eval(c);
                    if (res == exit)
                        return null;
                    return res;
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
                    var res = body.Eval(c);
                    if (res == exit)
                        return null;
                    return res;
                }

                return exit;
            };

            PlasticMacro @else = (c, a) =>
            {
                var last = c["last"];
                if (last != exit)
                    return last;

                var body = a[0];

                var res = body.Eval(c);
                if (res == exit)
                    return null;

                return res;
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
                    c.Declare(v.Value, element);
                    body.Eval(c);
                }
                return result;
            };

            PlasticMacro func = (_, a) =>
            {      
                var Args = a.Take(a.Length - 1).Select(arg =>
                {
                    var identifier = arg as Identifier;
                    if (identifier != null)
                    {
                        return new Argument(identifier.Value, ArgumentType.Value);
                    }
                    var dot = arg as BinaryExpression;
                    if (dot != null)
                    {
                        var id = dot.Left as Identifier;
                        var type = dot.Right as Identifier;
                        var argType = ArgumentType.Value;
                        if (type.Value == "ref")
                            argType = ArgumentType.Expression;

                        return new Argument(id.Value, argType);
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
                        var invocationScope =  new PlasticContextImpl(callingContext);
                        for (var i = 0; i < args.Length; i++)
                        {
                            var arg = Args[i];
                            if (arg.Type == ArgumentType.Expression)
                            {
                                //copy args from caller to this context
                                invocationScope.Declare(arg.Name, args[i]);
                            }
                            else if (arg.Type == ArgumentType.Value)
                            {
                                var value = args[i].Eval(callingContext);
                                invocationScope.Declare(arg.Name, value);
                            }
                        }

                        var m = Body.Eval(invocationScope);
                        return m;
                    }
                    //partial application
                    var partialArgs = args.ToArray();

                    PlasticMacro partial = (ctx, pargs) => op(ctx, partialArgs.Union(pargs).ToArray());

                    return partial;
                };
                return op;
            };


            PlasticMacro @class = (c, a) =>
            {
                var body = a.Last();
                PlasticMacro f = (ctx, args) =>
                {
                    var thisContext = new PlasticContextImpl(c);

                    for (var i = 0; i < a.Length - 1; i++)
                    {
                        var argName = a[i] as Identifier; //TODO: add support for expressions and partial appl
                        thisContext.Declare(argName.Value, args[i].Eval(ctx));
                    }

                    var self = new PlasticObject(thisContext);
                    thisContext.Declare("this", self);
                    body.Eval(thisContext);

                    return self;
                };
                return f;
            };

            PlasticMacro mixin = (c, a) =>
            {
                var body = a.Last();
                PlasticMacro f = (ctx, args) =>
                {
                    var thisContext = ctx;

                    for (var i = 0; i < a.Length - 1; i++)
                    {
                        var argName = a[i] as Identifier; //TODO: add support for expressions and partial appl
                        thisContext.Declare(argName.Value, args[i].Eval(ctx));
                    }

                    body.Eval(thisContext);

                    return null;
                };
                return f;
            };

            PlasticMacro @using = (c, a) =>
            {
                var arg = a.First();
                var path = arg.ToString().Replace(" ", "");
                Type type = Type.GetType(path);
                var id = (arg as Identifier ?? (arg as BinaryExpression).Right as Identifier).Value;

                c.Declare(id,type);

                return type;
            };

            PlasticMacro eval = (c, a) =>
            {
                var code = a.First().Eval(c) as string;
                var res = Run(code, c);
                return res;
            };

            PlasticMacro assign = (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                var value = right.Eval(c);
                var dot = left as BinaryExpression;
                var assignee = left as Identifier;

                if (assignee != null)
                {
                    c[assignee.Value] = value;
                }

                if (dot != null)
                {
                    var obj = dot.Left.Eval(c) as PlasticObject;
                    var memberId = dot.Right as Identifier;
                    obj[memberId.Value] = value;
                }

                return value;
            };

            PlasticMacro def = (c, a) =>
            {
                var left = a.ElementAt(0) as Identifier;
                var right = a.ElementAt(1);

                var value = right.Eval(context);
                context.Declare(left.Value, value);
                return value;
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
            context.Declare("mixin", mixin);
            context.Declare("class", @class);
            context.Declare("using", @using);
            context.Declare("eval", eval);
            context.Declare("assign", assign);
            context.Declare("def", def);
            
            BootstrapLib(context);

            return context;
        }
    }
}