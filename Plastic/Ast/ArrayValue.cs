using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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