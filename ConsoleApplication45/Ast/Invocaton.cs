using System;
using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class Invocaton : IExpression
    {
        public Invocaton(IExpression head, IEnumerable<IExpression> args)
        {
            Head = head;
            Args = args.ToArray();
        }

        public IExpression[] Args { get; set; }
        public IExpression Head { get; set; }

        public object Eval(PlasticContext context)
        {
            var target = Head.Eval(context);
            var interop = target as PlasticInterop;
            var expressions = target as IEnumerable<IExpression>;
            if (expressions != null)
            {
                object res = null;
                foreach (var item in expressions)
                {
                    var i = item.Eval(context);
                    var func = i as PlasticInterop;
                    res = Invoke(context, func);
                }
                return res;
            }

            if (interop != null)
            {
                return Invoke(context, interop);
            }

            throw new NotImplementedException();
        }

        private object Invoke(PlasticContext context, PlasticInterop interop)
        {
            var first = Args.First();
            object[] args = null;
            if (first is TupleValue)
            {
                args = (first as TupleValue).Items.Select(i => i.Eval(context)).ToArray();
            }

            return interop(args);
        }
    }
}