using System.Globalization;

namespace PlasticLang.Ast
{
    public class Number : IExpression
    {
        public static readonly Number One = new Number(1m);

        public Number(decimal numb)
        {
            Value = numb;
        }

        public Number(string numb)
        {
            Value = decimal.Parse(numb, NumberFormatInfo.InvariantInfo);
        }

        public decimal Value { get; private set; }

        public object Eval(PlasticContext context)
        {
            return context.Number(this);
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}