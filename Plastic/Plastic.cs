using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlasticLang.Ast;
using Sprache;

namespace PlasticLang
{
    public static class Plastic
    {
        public static void Run(string code)
        {
            var context = SetupCoreSymbols().Result;
            var userContext = new PlasticContextImpl(context);
            Run(code, userContext);
        }

        public static Task<object> Run(string code, PlasticContext context)
        {
            var res = PlasticParser.Statements.Parse(code);
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

        public static async Task<PlasticContext> SetupCoreSymbols()
        {
            var exit = new object();
            var context = new PlasticContextImpl();
            PlasticMacro print = async (c, a) =>
            {
                var obj = await a.First().Eval(c);
                var args = a.Skip(1).Select(async o => await o.Eval(c)).ToArray();
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

            PlasticMacro @while = async (c, a) =>
            {
                var result = exit;
                var cond = a[0];
                var body = a[1];

                while ((bool) await cond.Eval(c))
                {
                    result =await body.Eval(c);
                }

                return result;
            };

            PlasticMacro @if = async (c, a) =>
            {
                var cond = a[0];
                var body = a[1];

                if ((bool) await cond.Eval(c))
                {
                    var res = await body.Eval(c);
                    if (res == exit)
                        return null;
                    return res;
                }

                return exit;
            };

            PlasticMacro @elif = async (c, a) =>
            {
                var last = c["last"];
                if (last != exit)
                    return last;

                var cond = a[0];
                var body = a[1];

                if ((bool) await cond.Eval(c))
                {
                    var res = await body.Eval(c);
                    if (res == exit)
                        return null;
                    return res;
                }

                return exit;
            };

            PlasticMacro @else = async (c, a) =>
            {
                var last = c["last"];
                if (last != exit)
                    return last;

                var body = a[0];

                var res = await body.Eval(c);
                if (res == exit)
                    return null;

                return res;
            };

            PlasticMacro each = async (c, a) =>
            {
                var v = a[0] as Symbol;
                var body = a[2];

                var enumerable = await a[1].Eval(c) as IEnumerable;
                if (enumerable == null)
                    return exit;

                object result = null;
                foreach (var element in enumerable)
                {
                    c.Declare(v.Value, element);
                    result = await body.Eval(c);
                }
                return result;
            };

            PlasticMacro func = (_, a) =>
            {
                var argsMinusOne = a.Take(a.Length - 1).Select(arg =>
                {
                    var symbol = arg as Symbol;
                    if (symbol != null)
                    {
                        if (!symbol.Value.StartsWith("@"))
                            return new Argument(symbol.Value, ArgumentType.Value);
                        return new Argument(symbol.Value.Substring(1), ArgumentType.Expression);
                    }

                    throw new NotSupportedException();
                }).ToArray();
                var body = a.Last();

                PlasticMacro op = null;
                op = async (callingContext, args) =>
                {
                    //full application
                    if (args.Length >= argsMinusOne.Length)
                    {
                        //create context for this invocation
                        var invocationScope = new PlasticContextImpl(callingContext);
                        var arguments = new List<object>();
                        for (var i = 0; i < args.Length; i++)
                        {
                            var arg = argsMinusOne[i];
                            if (arg.Type == ArgumentType.Expression)
                            {
                                //copy args from caller to this context
                                var value = args[i];
                                invocationScope.Declare(arg.Name, value);
                                arguments.Add(value);
                            }
                            else if (arg.Type == ArgumentType.Value)
                            {
                                var value = await args[i].Eval(callingContext);
                                invocationScope.Declare(arg.Name, value);
                                arguments.Add(value);
                            }
                            invocationScope.Declare("args", arguments.ToArray());
                        }

                        var m = await body.Eval(invocationScope);
                        return m;
                    }
                    //partial application
                    var partialArgs = args.ToArray();

                    PlasticMacro partial = (ctx, pargs) => op(ctx, partialArgs.Union(pargs).ToArray());

                    return Task.FromResult((object)partial);
                };
                return Task.FromResult((object)op);
            };


            PlasticMacro @class = (c, a) =>
            {
                var body = a.Last();
                PlasticMacro f = async (ctx, args) =>
                {
                    var thisContext = new PlasticContextImpl(c);

                    for (var i = 0; i < a.Length - 1; i++)
                    {
                        var argName = a[i] as Symbol; //TODO: add support for expressions and partial appl
                        var arg = await args[i].Eval(ctx);
                        thisContext.Declare(argName.Value, arg);
                    }

                    var self = new PlasticObject(thisContext);
                    thisContext.Declare("this", self);
                    await body.Eval(thisContext);

                    return self;
                };
                return Task.FromResult((object)f);
            };

            PlasticMacro mixin = (c, a) =>
            {
                var body = a.Last();
                PlasticMacro f = async (ctx, args) =>
                {
                    var thisContext = ctx;

                    for (var i = 0; i < a.Length - 1; i++)
                    {
                        var argName = a[i] as Symbol; //TODO: add support for expressions and partial appl
                        thisContext.Declare(argName.Value, await args[i].Eval(ctx));
                    }

                    await body.Eval(thisContext);

                    return null;
                };
                return Task.FromResult((object)f);
            };

            PlasticMacro @using = (c, a) =>
            {
                var path = a.First() as StringLiteral;
                var type = Type.GetType(path.Value);
                return Task.FromResult((object)type);
            };

            PlasticMacro eval = async (c, a) =>
            {
                var code = await a.First().Eval(c) as string;
                var res = Run(code, c);
                return res;
            };

            PlasticMacro assign = async (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                var value = await right.Eval(c);

                var assignee = left as Symbol;

                if (assignee != null)
                {
                    c[assignee.Value] = value;
                }

                var dot = left as ListValue;
                if (dot != null)
                {
                    var obj = await dot.Args.ElementAt(0).Eval(c) as PlasticObject;
                    var memberId = dot.Args.ElementAt(1) as Symbol;
                    obj[memberId.Value] = value;
                }

                var tuple = left as TupleValue;
                var arr = value as TupleInstance;
                if (tuple != null)
                {
                    Func<TupleValue, TupleInstance, bool> match = null;
                    match = (t, values) =>
                    {
                        if (values == null)
                            return false;

                        if (t.Items.Length != values.Items.Length)
                            return false;

                        for (var i = 0; i < values.Items.Length; i++)
                        {
                            var l = t.Items[i];
                            var r = values.Items[i];

                            if (l is Symbol) //left is symbol, assign a value to it..
                            {
                                c[(l as Symbol).Value] = r;
                            }
                            else if (l is TupleValue)
                            {
                                if (r is TupleInstance)
                                {
                                    //right is a sub tuple, recursive match
                                    var subMatch = match(l as TupleValue, r as TupleInstance);
                                    if (!subMatch)
                                        return false;
                                }
                                else
                                {
                                    //left is tuple, right is not. just exit
                                    return false;
                                }
                            }
                            else
                            {
                                //left is a value, compare to right
                                var lv = l.Eval(c);
                                if (lv == null)
                                {
                                    if (r != null)
                                    {
                                        return false;
                                    }
                                }
                                else if (!lv.Equals(r))
                                {
                                    return false;
                                }
                            }
                        }

                        return true;
                    };

                    return match(tuple, arr);
                }

                return value;
            };

            PlasticMacro def = async (c, a) =>
            {
                var left = a.ElementAt(0) as Symbol;
                var right = a.ElementAt(1);

                var value = await right.Eval(c);
                context.Declare(left.Value, value);
                return value;
            };

            PlasticMacro add = async (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic)  await left.Eval(c) + (dynamic) await right.Eval(c);
            };

            PlasticMacro sub = async (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) - (dynamic) await right.Eval(c);
            };

            PlasticMacro mul = async (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c)*(dynamic) await right.Eval(c);
            };

            PlasticMacro div = async (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c)/(dynamic) await right.Eval(c);
            };

            PlasticMacro eq = async (c, a) =>
            {
                dynamic left = await a.ElementAt(0).Eval(c);
                dynamic right = await a.ElementAt(1).Eval(c);

                if (left == null)
                {
                    if (right != null)
                        return false;
                    return true;
                }
                if (right == null)
                    return false;

                if (left.GetType() != right.GetType())
                    return false;

                return left == right;
            };

            PlasticMacro neq = async (c, a) =>
            {
                var res = await eq(c, a);
                return !(bool)res;
            };

            PlasticMacro gt = async (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) > (dynamic) await right.Eval(c);
            };

            PlasticMacro gteq = async (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) >= (dynamic) await right.Eval(c);
            };

            PlasticMacro lt = async (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) < (dynamic) await right.Eval(c);
            };

            PlasticMacro lteq = async (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) <= (dynamic) await right.Eval(c);
            };

            PlasticMacro booland = async (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) && (dynamic) await right.Eval(c);
            };

            PlasticMacro boolor = async (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) || (dynamic) await right.Eval(c);
            };

            PlasticMacro not = async (c, a) =>
            {
                var exp = a.ElementAt(0);

                return !(dynamic) await exp.Eval(c);
            };

            PlasticMacro dotop = async (c, a) =>
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                var l = await left.Eval(c);

                var arr = l as object[];
                if (arr != null)
                {
                    var arrayContext = new ArrayContext(arr, c);
                    return await right.Eval(arrayContext);
                }

                var pobj = l as PlasticObject;
                if (pobj != null)
                {
                    return await right.Eval(pobj.Context);
                }

                var type = l as Type;
                if (type != null)
                {
                    var typeContext = new TypeContext(type, c);
                    return await right.Eval(typeContext);
                }


                var objContext = new InstanceContext(l, c);
                return await right.Eval(objContext);
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
            context.Declare("_not", not);
            //context.Declare("ActorSystem", actorSystem);


            BootstrapLib(context);

            return context;
        }
    }
}