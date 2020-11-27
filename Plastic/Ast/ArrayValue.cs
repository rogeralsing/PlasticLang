using System.Linq;

namespace PlasticLang.Ast
{
    public record ArrayValue(Syntax[] Items) : Syntax
    {
        public override string ToString()
        {
            return $"[{string.Join(",", Items.Select(i => i.ToString()))}]";
        }
    }
}