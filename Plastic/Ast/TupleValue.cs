using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public record TupleValue(Syntax[] Items) : Syntax
    {
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