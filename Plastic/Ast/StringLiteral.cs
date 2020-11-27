using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public record StringLiteral : Syntax
    {
        public StringLiteral(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }
    }
}