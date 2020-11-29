using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlasticLang.Ast;
using PlasticLang.Contexts;
using PlasticLang.Visitors;
using Sprache;

namespace PlasticLang
{
    public static class Plastic
    {
        private static readonly object Exit = new();

        public static object Run(string code)
        {
            var context = SetupCoreSymbols();
            var userContext = new PlasticContextImpl(context);
            return Run(code, userContext);
        }

        public static object Run(string code, PlasticContext context)
        {
            var res = PlasticParser.Statements.Parse(code);
            object result = default;
            foreach (var statement in res.Elements) result = statement.Eval(context);
            return result;
        }

        public static void BootstrapLib(PlasticContext context)
        {
            var lib = File.ReadAllText("core.pla");


            var libCode = PlasticParser.Statements.Parse(lib);
            object result = default;
            foreach (var statement in libCode.Elements) result = statement.Eval(context);
            var temp = result;
        }

        private static PlasticContext SetupCoreSymbols()
        {
            var context = new PlasticContextImpl();


            object? Def(PlasticContext c, Syntax[] a)
            {
                var left = a.Left() as Symbol;
                var right = a.Right();

                var value = right.Eval(c);
                context.Declare(left.Identity, value);
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
            context.Declare("exit", Exit);
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
            context.Declare("_gteq", GtEq);
            context.Declare("_lt", Lt);
            context.Declare("_lteq", LtEq);
            context.Declare("_band", LogicalAnd);
            context.Declare("_bor", LogicalOr);
            context.Declare("_dot", Dotop);
            context.Declare("_not", Not);
            //context.Declare("ActorSystem", actorSystem);


            BootstrapLib(context);

            return context;
        }

        private static object? Not(PlasticContext c, Syntax[] a)
        {
            return !(bool) a.Left().Eval(c);
        }

        private static object? Dotop(PlasticContext c, Syntax[] a)
        {
            var left = a.Left();
            var right = a.Right();

            var l = left.Eval(c);

            switch (l)
            {
                case object[] arr:
                {
                    var arrayContext = new ArrayContext(arr, c);
                    return right.Eval(arrayContext);
                }
                case PlasticObject pobj:
                    return right.Eval(pobj.Context);
                case Type type:
                {
                    var typeContext = new ClrTypeContext(type, c);
                    return right.Eval(typeContext);
                }
                default:
                {
                    var objContext = new ClrInstanceContext(l, c);
                    return right.Eval(objContext);
                }
            }
        }

        private static object? Mixin(PlasticContext c, Syntax[] a)
        {
            var body = a.Last();

            object? PlasticMacro(PlasticContext ctx, Syntax[] args)
            {
                var thisContext = ctx;

                for (var i = 0; i < a.Length - 1; i++)
                {
                    var argName = a[i] as Symbol; //TODO: add support for expressions and partial appl
                    thisContext.Declare(argName.Identity, args[i].Eval(ctx));
                }

                body.Eval(thisContext);

                return null;
            }

            return (PlasticMacro) PlasticMacro;
        }

        private static object? Print(PlasticContext c, Syntax[] a)
        {
            var obj = a.First().Eval(c);
            var source = a.Skip(1).ToArray();
            var args = new object[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                var v = source[i].Eval(c);
                args[i] = v;
            }

            if (args.Any())
                Console.WriteLine(obj.ToString(), args);
            else
                Console.WriteLine(obj);
            return obj;
        }

        private static object? While(PlasticContext c, Syntax[] a)
        {
            var result = Exit;
            var cond = a.Left();
            var body = a.Right();

            while ((bool) cond.Eval(c)) result = body.Eval(c);

            return result;
        }

        private static object? If(PlasticContext c, Syntax[] a)
        {
            var cond = a.Left();
            var body = a.Right();

            if ((bool) cond.Eval(c))
            {
                var res = body.Eval(c);
                if (res == Exit) return null;
                return res;
            }

            return Exit;
        }

        private static object? Elif(PlasticContext c, Syntax[] a)
        {
            var last = c["last"];
            if (last != Exit) return last;

            var cond = a.Left();
            var body = a.Right();

            if ((bool) cond.Eval(c))
            {
                var res = body.Eval(c);
                if (res == Exit) return null;
                return res;
            }

            return Exit;
        }

        private static object? Else(PlasticContext c, Syntax[] a)
        {
            var last = c["last"];
            if (last != Exit) return last;

            var body = a.Left();

            var res = body.Eval(c);
            if (res == Exit) return null;

            return res;
        }

        private static object? Each(PlasticContext c, Syntax[] a)
        {
            var v = a.Left() as Symbol;
            var body = a[2];

            if (!(a.Right().Eval(c) is IEnumerable enumerable)) return Exit;

            object result = null!;
            foreach (var element in enumerable)
            {
                c.Declare(v.Identity, element);
                result = body.Eval(c);
            }

            return result;
        }

        private static object Func(PlasticContext _, Syntax[] a)
        {
            var argsMinusOne = a.Take(a.Length - 1)
                .Select(arg =>
                {
                    var symbol = arg as Symbol;
                    if (symbol != null)
                    {
                        if (!symbol.Identity.StartsWith("@")) return new Argument(symbol.Identity, ArgumentType.Value);
                        return new Argument(symbol.Identity.Substring(1), ArgumentType.Expression);
                    }

                    throw new NotSupportedException();
                })
                .ToArray();
            var body = a.Last();

            object? Op(PlasticContext callingContext, Syntax[] args)
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
                            var value = args[i].Eval(callingContext);
                            invocationScope.Declare(arg.Name, value);
                            arguments.Add(value);
                        }

                        invocationScope.Declare("args", arguments.ToArray());
                    }

                    var m = body.Eval(invocationScope);
                    return m;
                }

                //partial application
                var partialArgs = args.ToArray();

                object? Partial(PlasticContext ctx, Syntax[] pargs)
                {
                    return Op(ctx, partialArgs.Union(pargs).ToArray());
                }

                return (PlasticMacro) Partial;
            }

            return (PlasticMacro) Op;
        }


        private static object Class(PlasticContext c, Syntax[] a)
        {
            var body = a.Last();

            object PlasticMacro(PlasticContext ctx, Syntax[] args)
            {
                var thisContext = new PlasticContextImpl(c);

                for (var i = 0; i < a.Length - 1; i++)
                {
                    var argName = a[i] as Symbol; //TODO: add support for expressions and partial appl
                    var arg = args[i].Eval(ctx);
                    thisContext.Declare(argName!.Identity, arg);
                }

                var self = new PlasticObject(thisContext);
                thisContext.Declare("this", self);
                body.Eval(thisContext);

                return self;
            }

            return (PlasticMacro) PlasticMacro!;
        }

        private static object? Using(PlasticContext c, Syntax[] a)
        {
            var path = a.First() as StringLiteral;
            var type = Type.GetType(path.Value);
            return type;
        }

        private static object? Eval(PlasticContext c, Syntax[] a)
        {
            var code = a.First().Eval(c) as string;
            var res = Run(code!, c);
            return res;
        }

        private static object? Assign(PlasticContext c, Syntax[] a)
        {
            var left = a.Left();
            var right = a.Right();

            var value = right.Eval(c);

            switch (left)
            {
                case Symbol assignee:
                    c[assignee.Identity] = value;
                    break;
                case ListValue dot:
                {
                    var obj = dot.Rest[0].Eval(c) as PlasticObject;
                    var memberId = dot.Rest[1] as Symbol;
                    obj![memberId!.Identity] = value;
                    break;
                }
            }


            if (left is not TupleValue tuple) return value;
            var arr = value as TupleInstance;
            return Match(tuple, arr!);

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
                            c[symbol.Identity] = r;
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
                            if (lv is null)
                            {
                                if (r is not null) return false;
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
        }

        private static object? Add(PlasticContext c, Syntax[] a)
        {
            return (dynamic) a.Left().Eval(c) + (dynamic) a.Right().Eval(c);
        }

        private static object? Sub(PlasticContext c, Syntax[] a)
        {
            return (dynamic) a.Left().Eval(c) - (dynamic) a.Right().Eval(c);
        }

        private static object? Mul(PlasticContext c, Syntax[] a)
        {
            return (dynamic) a.Left().Eval(c) * (dynamic) a.Right().Eval(c);
        }

        private static object? Div(PlasticContext c, Syntax[] a)
        {
            return (dynamic) a.Left().Eval(c) / (dynamic) a.Right().Eval(c);
        }

        private static object? Eq(PlasticContext c, Syntax[] a)
        {
            var left = a.Left().Eval(c);
            var right = a.Right().Eval(c);

            if (left == null && right == null) return true;

            if (left == null || right == null) return false;

            if (left.GetType() != right.GetType()) return false;

            return left == right;
        }

        private static object? Neq(PlasticContext c, Syntax[] a)
        {
            return !((bool) Eq(c, a))!;
        }

        private static object? Gt(PlasticContext c, Syntax[] a)
        {
            return (dynamic) a.Left().Eval(c) > (dynamic) a.Right().Eval(c);
        }

        private static object? GtEq(PlasticContext c, Syntax[] a)
        {
            return (dynamic) a.Left().Eval(c) >= (dynamic) a.Right().Eval(c);
        }

        private static object? Lt(PlasticContext c, Syntax[] a)
        {
            return (dynamic) a.Left().Eval(c) < (dynamic) a.Right().Eval(c);
        }

        private static object? LtEq(PlasticContext c, Syntax[] a)
        {
            return (dynamic) a.Left().Eval(c) <= (dynamic) a.Right().Eval(c);
        }

        private static object? LogicalAnd(PlasticContext c, Syntax[] a)
        {
            return (dynamic) a.Left().Eval(c) && (dynamic) a.Right().Eval(c);
        }

        private static object? LogicalOr(PlasticContext c, Syntax[] a)
        {
            return (dynamic) a.Left().Eval(c) || (dynamic) a.Right().Eval(c);
        }
    }
}