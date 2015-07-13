using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class TupleValue : IExpression
    {
        public TupleValue(IEnumerable<IExpression> items)
        {
            Items = items.ToArray();
        }

        public IExpression[] Items { get; set; }

        public object Eval(PlasticContext context)
        {
            return Items;
        }
    }
}