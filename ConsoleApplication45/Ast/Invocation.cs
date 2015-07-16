using System;
using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class Invocation : IExpression
    {
        public Invocation(IExpression head, TupleValue args, IExpression body)
        {
            Head = head;
            var tmp = args != null ? args.Items : new IExpression[0];

            if (body != null)
            {
                var oneBody = Enumerable.Repeat(body, 1).ToArray();
                tmp = tmp.Union(oneBody).ToArray();
            }
            Args = tmp.ToArray();
        }

        public IExpression[] Args { get; set; }
        public IExpression Head { get; set; }

        public object Eval(PlasticContext context)
        {
            var target = Head.Eval(context);
            var macro = target as PlasticMacro;
            var expressions = target as IEnumerable<IExpression>;
            var expression = target as IExpression;
            var array = target as object[];
            if (expressions != null)
            {
                return InvokeMulti(context, expressions);
            }

            if (macro != null)
            {
                return InvokeMacro(context, macro);
            }

            if (expression != null)
            {
                return expression.Eval(context);
            }

            if (array != null)
            {
                var index = (int) (decimal) Args.First().Eval(context);
                return array[index];
            }
            throw new NotImplementedException();
        }

        private object InvokeMulti(PlasticContext context, IEnumerable<IExpression> expressions)
        {
            object res = null;
            foreach (var item in expressions)
            {
                var i = item.Eval(context);
                var macro = i as PlasticMacro;
                if (macro != null)
                    res = InvokeMacro(context, macro);
            }
            return res;
        }

        private object InvokeMacro(PlasticContext context, PlasticMacro macro)
        {
            var ctx = context.ChildContext();
            var args = Args;
            var res = macro(ctx, args);
            context.Declare("last", res);
            return res;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", Head, string.Join(",", Args.Select(a => a.ToString())));
        }
    }
}