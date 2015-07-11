namespace PlasticLangLabb1
{
    internal class QuotedString : IExpression
    {
        public QuotedString(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}