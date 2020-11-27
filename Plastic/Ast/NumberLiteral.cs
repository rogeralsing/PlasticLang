using System.Globalization;
using System.Threading.Tasks;


namespace PlasticLang.Ast
{
    public record NumberLiteral : Syntax
    {
        public static readonly NumberLiteral One = new(1m);

        public NumberLiteral(decimal numb)
        {
            Value = numb;
        }

        public NumberLiteral(string numb)
        {
            Value = decimal.Parse(numb, NumberFormatInfo.InvariantInfo);
        }

        public decimal Value { get; }

        public ValueTask<object> Eval(PlasticContext context) => context.Number(this);

        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
    }
}