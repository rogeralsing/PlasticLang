using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public class ArrayValue : IExpression
    {
        public ArrayValue(IEnumerable<IExpression> items)
        {
            Items = items.ToArray();
        }

        public IExpression[] Items { get; set; }

        public async Task<object> Eval(PlasticContext context)
        {
            var items = new object[Items.Length];
            for (int i = 0; i < Items.Length; i++)
            {
                items[i] = await Items[i].Eval(context);
            }
            return items;
        }

        public override string ToString()
        {
            return $"[{string.Join(",", Items.Select(i => i.ToString()))}]";
        }
    }
}