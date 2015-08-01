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
            return new TupleInstance(Items.Select(i => i.Eval(context)));
        }

        public override string ToString()
        {
            return string.Format("({0})", string.Join(",", Items.Select(i => i.ToString())));
        }
    }

    public class TupleInstance
    {
        public TupleInstance(params object[] items)
        {
            Items = items;
        }

        public TupleInstance(IEnumerable<object> items)
        {
            Items = items.ToArray();
        }

        public object[] Items { get; set; }

        public override string ToString()
        {
            return string.Format("({0})", string.Join(",", Items.Select(i => i.ToString())));
        }
    }
}