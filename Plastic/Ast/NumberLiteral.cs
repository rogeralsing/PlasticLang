using System.Globalization;

namespace PlasticLang.Ast
{
    public record NumberLiteral(decimal Value) : Syntax
    {
        public static readonly NumberLiteral One = new(1m);

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}