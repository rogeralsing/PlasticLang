using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public Task<object> Eval(PlasticContext context)
        {
            var res = new TupleInstance(Items.Select(i => i.Eval(context)));
            return Task.FromResult((object) res);
        }

        public override string ToString()
        {
            return $"({string.Join(",", Items.Select(i => i.ToString()))})";
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