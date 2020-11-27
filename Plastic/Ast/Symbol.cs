

using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public record Symbol(string Value) : Syntax
    {
        public override string ToString()
        {
            return Value;
        }
    }
}