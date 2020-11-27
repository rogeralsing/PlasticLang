using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public record ArrayValue : Syntax
    {
        public ArrayValue(IEnumerable<Syntax> items)
        {
            Items = items.ToArray();
        }

        public Syntax[] Items { get; }

        public override async ValueTask<object> Eval(PlasticContext context)
        {
            var items = new object[Items.Length];
            for (var i = 0; i < Items.Length; i++) items[i] = await Items[i].Eval(context);
            return items;
        }

        public override string ToString()
        {
            return $"[{string.Join(",", Items.Select(i => i.ToString()))}]";
        }
    }
}