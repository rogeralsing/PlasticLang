using System.Collections.Generic;
using System.Linq;

namespace PlasticLang.Ast
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