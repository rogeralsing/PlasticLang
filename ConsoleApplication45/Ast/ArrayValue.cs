using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class ArrayValue : IExpression
    {
        public ArrayValue(IEnumerable<IExpression> items)
        {
            Items = items.ToArray();
        }

        public IExpression[] Items { get; set; }

        public object Eval(PlasticContext context)
        {
            return Items.Select(i => i.Eval(context)).ToArray();
        }
    }
}