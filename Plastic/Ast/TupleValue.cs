using System.Collections.Generic;
using System.Linq;

namespace PlasticLang.Ast
{
    public class TupleValue : IExpression
    {
        public TupleValue(params IExpression[] items)
        {
            Items = items;
        }

        public TupleValue(IEnumerable<IExpression> items)
        {
            Items = items.ToArray();
        }

        public IExpression[] Items { get; set; }

        public object Eval(PlasticContext context)
        {
            return Items.Select(i => i.Eval(context)).ToArray();
        }

        public override string ToString()
        {
            return string.Format("({0})", string.Join(",", Items.Select(i => i.ToString())));
        }
    }
}