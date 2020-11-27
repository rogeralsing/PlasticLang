using System.Globalization;
using System.Threading.Tasks;


namespace PlasticLang.Ast
{
    public record NumberLiteral(decimal Value) : Syntax
    {
        public static readonly NumberLiteral One = new(1m);

        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
    }
}