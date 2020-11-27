using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public record TupleValue : Syntax
    {
        public TupleValue(IEnumerable<Syntax> items)
        {
            Items = items.ToArray();
        }

        public Syntax[] Items { get; }

        public async ValueTask<object> Eval(PlasticContext context)
        {
            var items = new object[Items.Length];
            for (var i = 0; i < Items.Length; i++)
            {
                var v = await Evaluator.Eval(Items[i],context);
                items[i] = v;
            }

            var res = new TupleInstance(items);
            return res;
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

        public object[] Items { get; }

        public override string ToString()
        {
            return $"({string.Join(",", Items.Select(i => i.ToString()))})";
        }
    }
}