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

        public Task<object> Eval(PlasticContext context)
        {
            var res = Items.Select(i => i.Eval(context)).ToArray();
            return Task.FromResult((object) res);
        }

        public override string ToString()
        {
            return $"[{string.Join(",", Items.Select(i => i.ToString()))}]";
        }
    }
}