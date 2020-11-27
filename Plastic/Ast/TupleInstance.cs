using System.Linq;

namespace PlasticLang.Ast
{
    public record TupleInstance(params object[] Items)
    {
        public override string ToString()
        {
            return $"({string.Join(",", Items.Select(i => i.ToString()))})";
        }
    }
}