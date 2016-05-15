using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public class StringLiteral : IExpression ,IStringLiteral
    {
        public StringLiteral(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public Task<object> Eval(PlasticContext context)
        {
            return context.QuotedString(this);
        }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }
    }
}