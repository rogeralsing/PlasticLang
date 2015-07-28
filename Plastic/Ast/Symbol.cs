namespace PlasticLang.Ast
{
    public class Symbol : IExpression , IStringLiteral
    {
        public Symbol(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public object Eval(PlasticContext context)
        {
            return context[Value];
        }

        public override string ToString()
        {
            return Value;
        }
    }
}