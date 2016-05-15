using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public class Symbol : IExpression , IStringLiteral
    {
        public Symbol(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public Task<object> Eval(PlasticContext context)
        {
            return Task.FromResult(context[Value]);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}