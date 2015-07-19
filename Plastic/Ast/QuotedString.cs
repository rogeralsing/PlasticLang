namespace PlasticLang.Ast
{
    public class QuotedString : IExpression ,IStringLiteral
    {
        public QuotedString(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public object Eval(PlasticContext context)
        {
            return context.QuotedString(this);
        }

        public override string ToString()
        {
            return string.Format("\"{0}\"", Value);
        }
    }
}