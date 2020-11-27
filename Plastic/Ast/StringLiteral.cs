using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public record StringLiteral : Syntax, IStringLiteral
    {
        public StringLiteral(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public ValueTask<object> Eval(PlasticContext context)
        {
            return context.QuotedString(this);
        }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }
    }
}