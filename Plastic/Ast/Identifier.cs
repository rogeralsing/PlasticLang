namespace PlasticLang.Ast
{
    public class Identifier : IExpression , IStringLiteral
    {
        public Identifier(string value)
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