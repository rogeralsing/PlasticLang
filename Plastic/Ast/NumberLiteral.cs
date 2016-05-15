using System.Globalization;
using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public class NumberLiteral : IExpression
    {
        public static readonly NumberLiteral One = new NumberLiteral(1m);

        public NumberLiteral(decimal numb)
        {
            Value = numb;
        }

        public NumberLiteral(string numb)
        {
            Value = decimal.Parse(numb, NumberFormatInfo.InvariantInfo);
        }

        public decimal Value { get; private set; }

        public Task<object> Eval(PlasticContext context)
        {
            return context.Number(this);
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}