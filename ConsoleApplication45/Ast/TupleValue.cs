using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace PlasticLangLabb1.Ast
{
    public class TupleValue : IExpression
    {
        public TupleValue(IEnumerable<IExpression> items)
        {
            Items = items.ToArray();
        }

        public IExpression[] Items { get; set; }
    }
}
