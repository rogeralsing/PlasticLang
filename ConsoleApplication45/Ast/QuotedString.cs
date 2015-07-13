namespace PlasticLangLabb1.Ast
{
    public class QuotedString : IExpression
    {
        public QuotedString(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public object Eval(PlasticContext context)
        {
            return Value;
        }
    }
}