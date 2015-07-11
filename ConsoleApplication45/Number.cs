using System.Globalization;

namespace PlasticLangLabb1
{
    public interface IExpression
    {
    }

    internal class Number : IExpression
    {
        public Number(string numb)
        {
            Value = decimal.Parse(numb, NumberFormatInfo.InvariantInfo);
        }

        public decimal Value { get; private set; }
    }
}