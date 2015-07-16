using System.Globalization;

namespace PlasticLang.Ast
{
    public class Number : IExpression
    {
        public Number(string numb)
        {
            Value = decimal.Parse(numb, NumberFormatInfo.InvariantInfo);
        }

        public decimal Value { get; private set; }

        public object Eval(PlasticContext context)
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}