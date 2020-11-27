namespace PlasticLang.Ast
{
    public record StringLiteral(string Value) : Syntax
    {
        public override string ToString() => $"\"{Value}\"";
    }
}