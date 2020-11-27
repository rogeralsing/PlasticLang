

using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public record Symbol : Syntax, IStringLiteral
    {
        public Symbol(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public ValueTask<object> Eval(PlasticContext context) => ValueTask.FromResult(context[Value]);

        public override string ToString()
        {
            return Value;
        }
    }
}