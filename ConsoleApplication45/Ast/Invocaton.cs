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
            if (interop != null)
            {
                var first = Args.First();
                object[] args = null;
                if (first is TupleValue)
                {
                    args = (first as TupleValue).Items.Select(i => i.Eval(context)).ToArray();
                }

                return interop(args);
            }

            throw new NotImplementedException();
        }
    }
}