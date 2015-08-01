using System;
using System.Collections;
using System.Collections.Generic;
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
            Console.WriteLine(res);
            return res.Eval(context);
        }

        public static void BootstrapLib(PlasticContext context)
        {
            var lib = @"
for := func (@init , @guard, @step, @body)
{
    init()
    while(guard())
    {
        body()
        step()
    }
}

repeat := func (times, @body)
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


switch :=  func(exp, @body)
{
    matched = false;
    case := func (value, @caseBody)
    {   
        if (exp == value)
        {
            caseBody();
            matched = true;
        }
    }
    default := func (@defaultBody)
    {
        if (matched == false)
        {
            defaultBody();
        }
    }
    body();
}

quote := func(@q)
{
    q
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
                var v = a[0] as Symbol;
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
                    var symbol = arg as Symbol;
                    if (symbol != null)
                    {
                        if(!symbol.Value.StartsWith("@"))
                            return new Argument(symbol.Value, ArgumentType.Value);
                        return new Argument(symbol.Value.Substring(1), ArgumentType.Expression);
                    }

                    throw new NotSupportedException();
                }).ToArray();
                var Body = a.Last();

                PlasticMacro op = null;
                op = (callingContext, args) =>
                {
                    //full application
                    if (args.Length >= Args.Length)
                    {
                        //create context for this invocation
                        var invocationScope =  new PlasticContextImpl(callingContext);
                        var arguments = new List<object>();
                        for (var i = 0; i < args.Length; i++)
                        {
                            var arg = Args[i];
                            if (arg.Type == ArgumentType.Expression)
                            {
                                //copy args from caller to this context
                                var value = args[i];
                                invocationScope.Declare(arg.Name, value);
                                arguments.Add(value);
                            }
                            else if (arg.Type == ArgumentType.Value)
                            {
                                var value = args[i].Eval(callingContext);
                                invocationScope.Declare(arg.Name, value);
                                arguments.Add(value);
                            }
                            invocationScope.Declare("args", arguments.ToArray());
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
                        var argName = a[i] as Symbol; //TODO: add support for expressions and partial appl
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
                        var argName = a[i] as Symbol; //TODO: add support for expressions and partial appl
                        thisContext.Declare(argName.Value, args[i].Eval(ctx));
                    }

                    body.Eval(thisContext);

                    return null;
                };
                return f;
            };

            PlasticMacro @using = (c, a) =>
            {
                var path = a.First() as StringLiteral;
                Type type = Type.GetType(path.Value);
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
                
                var assignee = left as Symbol;

                if (assignee != null)
                {
                    c[assignee.Value] = value;
                }

                var dot = left as ListValue;
                if (dot != null)
                {
                    var obj = dot.Args.ElementAt(0).Eval(c) as PlasticObject;
                    var memberId = dot.Args.ElementAt(1) as Symbol;
                    obj[memberId.Value] = value;
                }

                return value;
            };

            PlasticMacro def = (c, a) =>
            {
                var left = a.ElementAt(0) as Symbol;
                var right = a.ElementAt(1);

                var value = right.Eval(c);
                context.Declare(left.Value, value);
                return value;
            };

            PlasticMacro add = (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return ((dynamic)left.Eval(c)) + ((dynamic)right.Eval(c));                
            };

            PlasticMacro sub = (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return ((dynamic)left.Eval(c)) - ((dynamic)right.Eval(c));
            };

            PlasticMacro mul = (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return ((dynamic)left.Eval(c)) * ((dynamic)right.Eval(c));
            };

            PlasticMacro div = (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return ((dynamic)left.Eval(c)) / ((dynamic)right.Eval(c));
            };

            PlasticMacro eq = (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return ((dynamic)left.Eval(c)) == ((dynamic)right.Eval(c));
            };

            PlasticMacro neq = (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return ((dynamic)left.Eval(c)) != ((dynamic)right.Eval(c));
            };

            PlasticMacro gt = (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return ((dynamic)left.Eval(c)) > ((dynamic)right.Eval(c));
            };

            PlasticMacro gteq = (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return ((dynamic)left.Eval(c)) >= ((dynamic)right.Eval(c));
            };

            PlasticMacro lt = (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return ((dynamic)left.Eval(c)) < ((dynamic)right.Eval(c));
            };

            PlasticMacro lteq = (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return ((dynamic)left.Eval(c)) <= ((dynamic)right.Eval(c));
            };

            PlasticMacro booland = (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return ((dynamic)left.Eval(c)) && ((dynamic)right.Eval(c));
            };

            PlasticMacro boolor = (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return ((dynamic)left.Eval(c)) || ((dynamic)right.Eval(c));
            };

            PlasticMacro dotop = (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                var l = left.Eval(c);

                var arr = l as object[];
                if (arr != null)
                {
                    var arrayContext = new ArrayContext(arr, c);
                    return right.Eval(arrayContext);
                }

                var pobj = l as PlasticObject;
                if (pobj != null)
                {
                    return right.Eval(pobj.Context);
                }

                var type = l as Type;
                if (type != null)
                {
                    var typeContext = new TypeContext(type, c);
                    return right.Eval(typeContext);
                }


                var objContext = new InstanceContext(l, c);
                return right.Eval(objContext);
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
            context.Declare("_add", add);
            context.Declare("_sub", sub);
            context.Declare("_mul", mul);
            context.Declare("_div", div);
            context.Declare("_div", div);
            context.Declare("_eq", eq);
            context.Declare("_neq", neq);
            context.Declare("_gt", gt);
            context.Declare("_gteq", gteq);
            context.Declare("_lt", lt);
            context.Declare("_lteq", lteq);
            context.Declare("_band", booland);
            context.Declare("_bor", boolor);
            context.Declare("_dot", dotop);
           
            
            BootstrapLib(context);

            return context;
        }
    }
}