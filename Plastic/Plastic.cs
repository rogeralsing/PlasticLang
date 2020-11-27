using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlasticLang.Ast;
using PlasticLang.Contexts;
using PlasticLang.Visitors;
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

        public static ValueTask<object> Run(string code, PlasticContext context)
        {
            var res = PlasticParser.Statements.Parse(code);
            ValueTask<object> result = default;
            foreach (var statement in res.Elements) result = statement.Eval(context);
            return result;
        }

        public static void BootstrapLib(PlasticContext context)
        {
            var lib = @"
for := func (@init , @guard, @step, @body) {
    init()
    while(guard()) {
        body()
        step()
    }
}

repeat := func (times, @body) {
    while(times >= 0) {
        body()
        times--
    }
}

LinkedList := class {
    Node := class (value) { next = null; }

    head := null;
    tail := null;
    add := func (value) {
        node := Node(value);
        if (head == null) {         
            head = node;
            tail = node;
        }
        else {
            tail.next =  node;
            tail = node;  
        }        
    }

    each := func (lambda) {
        current := head;
        while(current != null) {
            lambda(current.value);
            current = current.next;
        }
    }
}

Stack := class {
    Node := class (value,prev) { next = null; }

    head := null;
    tail := null;
    push := func (value) {
        node = Node(value,tail);
        if (head == null) {         
            head = node;
            tail = node;
        }
        else {
            tail.next =  node;
            tail = node;  
        }        
    }

    each := func (lambda) {
        current = tail;
        while(current != null) {
            lambda(current.value);
            current = current.prev;
        }
    }

    peek := func() {
        tail.value;
    }

    pop := func() {
        res = tail.value;
        tail = tail.prev;
        if (tail != null) {
            tail.next = null;
        }
        else {
            head = null;
        }
        res
    }
}


switch :=  func(exp, @body) {
    matched := false;
    case := func (value, @caseBody) {   
        if (exp == value) {
            caseBody();
            matched = true;
        }
    }
    default := func (@defaultBody) {
        if (matched == false) {
            defaultBody();
        }
    }
    body();
}

quote := func(@q) {
    q
}


";


            var libCode = PlasticParser.Statements.Parse(lib);
            ValueTask<object> result = default;
            foreach (var statement in libCode.Elements) result = statement.Eval(context);
            var temp = result;
        }

        private static object exit = new object();
        private static async ValueTask<PlasticContext> SetupCoreSymbols()
        {
            var context = new PlasticContextImpl();

           

            async ValueTask<object?> Def(PlasticContext c, Syntax[] a)
            {
                var left = a.ElementAt(0) as Symbol;
                var right = a.ElementAt(1);

                var value = await right.Eval(c);
                context.Declare(left.Value, value);
                return value;
            }

            

            context.Declare("print", Print);
            context.Declare("while", While);
            context.Declare("each", Each);
            context.Declare("if", If);
            context.Declare("elif", Elif);
            context.Declare("else", Else);
            context.Declare("true", true);
            context.Declare("false", false);
            context.Declare("null", null);
            context.Declare("exit", exit);
            context.Declare("func", Func);
            context.Declare("mixin", Mixin);
            context.Declare("class", Class);
            context.Declare("using", Using);
            context.Declare("eval", Eval);
            context.Declare("assign", Assign);
            context.Declare("def", Def);
            context.Declare("_add", Add);
            context.Declare("_sub", Sub);
            context.Declare("_mul", Mul);
            context.Declare("_div", Div);
            context.Declare("_div", Div);
            context.Declare("_eq", Eq);
            context.Declare("_neq", Neq);
            context.Declare("_gt", Gt);
            context.Declare("_gteq", Gteq);
            context.Declare("_lt", Lt);
            context.Declare("_lteq", Lteq);
            context.Declare("_band", Booland);
            context.Declare("_bor", Boolor);
            context.Declare("_dot", Dotop);
            context.Declare("_not", Not);
            //context.Declare("ActorSystem", actorSystem);


            BootstrapLib(context);

            return context;
        }

        private static async ValueTask<object?> Not(PlasticContext c, Syntax[] a)
        {
            var exp = a.ElementAt(0);

            return !(dynamic) await exp.Eval(c);
        }

        private static async ValueTask<object?> Dotop(PlasticContext c, Syntax[] a)
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
            if (pobj != null) return await right.Eval(pobj.Context);

            var type = l as Type;
            if (type != null)
            {
                var typeContext = new ClrTypeContext(type, c);
                return await right.Eval(typeContext);
            }


            var objContext = new ClrInstanceContext(l, c);
            return await right.Eval(objContext);
        }

        private static ValueTask<object?> Mixin(PlasticContext c, Syntax[] a)
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
            return ValueTask.FromResult((object) f);
        }
        
         static async ValueTask<object?> Print(PlasticContext c, Syntax[] a)
            {
                var obj = await a.First().Eval(c);
                var source = a.Skip(1).ToArray();
                var args = new object[source.Length];
                for (var i = 0; i < source.Length; i++)
                {
                    var v = await source[i].Eval(c);
                    args[i] = v;
                }

                if (args.Any())
                    Console.WriteLine(obj.ToString(), args);
                else
                    Console.WriteLine(obj);
                return obj;
            }

            static async ValueTask<object?> While(PlasticContext c, Syntax[] a)
            {
                var result = exit;
                var cond = a[0];
                var body = a[1];

                while ((bool) await cond.Eval(c)) result = await body.Eval(c);

                return result;
            }

            static async ValueTask<object?> If(PlasticContext c, Syntax[] a)
            {
                var cond = a[0];
                var body = a[1];

                if ((bool) await cond.Eval(c))
                {
                    var res = await body.Eval(c);
                    if (res == exit) return null;
                    return res;
                }

                return exit;
            }

            static async ValueTask<object?> Elif(PlasticContext c, Syntax[] a)
            {
                var last = c["last"];
                if (last != exit) return last;

                var cond = a[0];
                var body = a[1];

                if ((bool) await cond.Eval(c))
                {
                    var res = await body.Eval(c);
                    if (res == exit) return null;
                    return res;
                }

                return exit;
            }

            static async ValueTask<object?> Else(PlasticContext c, Syntax[] a)
            {
                var last = c["last"];
                if (last != exit) return last;

                var body = a[0];

                var res = await body.Eval(c);
                if (res == exit) return null;

                return res;
            }

            static async ValueTask<object?> Each(PlasticContext c, Syntax[] a)
            {
                var v = a[0] as Symbol;
                var body = a[2];

                if (!(await a[1].Eval(c) is IEnumerable enumerable)) return exit;

                object result = null;
                foreach (var element in enumerable)
                {
                    c.Declare(v.Value, element);
                    result = await body.Eval(c);
                }

                return result;
            }

            static ValueTask<object?> Func(PlasticContext _, Syntax[] a)
            {
                var argsMinusOne = a.Take(a.Length - 1)
                    .Select(arg =>
                    {
                        var symbol = arg as Symbol;
                        if (symbol != null)
                        {
                            if (!symbol.Value.StartsWith("@")) return new Argument(symbol.Value, ArgumentType.Value);
                            return new Argument(symbol.Value.Substring(1), ArgumentType.Expression);
                        }

                        throw new NotSupportedException();
                    })
                    .ToArray();
                var body = a.Last();

                async ValueTask<object?> Op(PlasticContext callingContext, Syntax[] args)
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

                    PlasticMacro partial = (ctx, pargs) => Op(ctx, partialArgs.Union(pargs).ToArray());

                    return ValueTask.FromResult((object) partial);
                }

                return ValueTask.FromResult((object) (PlasticMacro) Op);
            }


            static ValueTask<object?> Class(PlasticContext c, Syntax[] a)
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
                return ValueTask.FromResult((object) f);
            }

            static ValueTask<object?> Using(PlasticContext c, Syntax[] a)
            {
                var path = a.First() as StringLiteral;
                var type = Type.GetType(path.Value);
                return ValueTask.FromResult((object) type);
            }

            static async ValueTask<object?> Eval(PlasticContext c, Syntax[] a)
            {
                var code = await a.First().Eval(c) as string;
                var res = Run(code, c);
                return res;
            }

            static async ValueTask<object?> Assign(PlasticContext c, Syntax[] a)
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                var value = await right.Eval(c);

                switch (left)
                {
                    case Symbol assignee:
                        c[assignee.Value] = value;
                        break;
                    case ListValue dot:
                    {
                        var obj = await dot.Rest.ElementAt(0).Eval(c) as PlasticObject;
                        var memberId = dot.Rest.ElementAt(1) as Symbol;
                        obj[memberId.Value] = value;
                        break;
                    }
                }


                var tuple = left as TupleValue;
                var arr = value as TupleInstance;
                if (tuple == null) return value;

                bool Match(TupleValue t, TupleInstance values)
                {
                    if (values == null) return false;

                    if (t.Items.Length != values.Items.Length) return false;

                    for (var i = 0; i < values.Items.Length; i++)
                    {
                        var l = t.Items[i];
                        var r = values.Items[i];

                        switch (l)
                        {
                            //left is symbol, assign a value to it..
                            case Symbol symbol:
                                c[symbol.Value] = r;
                                break;
                            case TupleValue leftTuple when r is TupleInstance rightTuple:
                            {
                                //right is a sub tuple, recursive match
                                var subMatch = Match(leftTuple, rightTuple);
                                if (!subMatch) return false;
                                break;
                            }
                            case TupleValue leftTuple:
                                //left is tuple, right is not. just exit
                                return false;
                            default:
                            {
                                //left is a value, compare to right
                                var lv = l.Eval(c);
                                if (lv == null)
                                {
                                    if (r != null) return false;
                                }
                                else if (!lv.Equals(r))
                                {
                                    return false;
                                }

                                break;
                            }
                        }
                    }

                    return true;
                }

                return Match(tuple, arr);
            }
            
            static async ValueTask<object?> Add(PlasticContext c, Syntax[] a)
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) + (dynamic) await right.Eval(c);
            }

            static async ValueTask<object?> Sub(PlasticContext c, Syntax[] a)
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) - (dynamic) await right.Eval(c);
            }

            static async ValueTask<object?> Mul(PlasticContext c, Syntax[] a)
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) * (dynamic) await right.Eval(c);
            }

            static async ValueTask<object?> Div(PlasticContext c, Syntax[] a)
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) / (dynamic) await right.Eval(c);
            }

            static async ValueTask<object?> Eq(PlasticContext c, Syntax[] a)
            {
                dynamic left = await a.ElementAt(0).Eval(c);
                dynamic right = await a.ElementAt(1).Eval(c);

                if (left == null)
                {
                    if (right != null) return false;
                    return true;
                }

                if (right == null) return false;

                if (left.GetType() != right.GetType()) return false;

                return left == right;
            }

            static async ValueTask<object?> Neq(PlasticContext c, Syntax[] a)
            {
                var res = await Eq(c, a);
                return !(bool) res;
            }

            static async ValueTask<object?> Gt(PlasticContext c, Syntax[] a)
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) > (dynamic) await right.Eval(c);
            }

            static async ValueTask<object?> Gteq(PlasticContext c, Syntax[] a)
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) >= (dynamic) await right.Eval(c);
            }

            static async ValueTask<object?> Lt(PlasticContext c, Syntax[] a)
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) < (dynamic) await right.Eval(c);
            }

            static async ValueTask<object?> Lteq(PlasticContext c, Syntax[] a)
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) <= (dynamic) await right.Eval(c);
            }

            static async ValueTask<object?> Booland(PlasticContext c, Syntax[] a)
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) && (dynamic) await right.Eval(c);
            }

            static async ValueTask<object?> Boolor(PlasticContext c, Syntax[] a)
            {
                var left = a.ElementAt(0);
                var right = a.ElementAt(1);

                return (dynamic) await left.Eval(c) || (dynamic) await right.Eval(c);
            }
    }
}