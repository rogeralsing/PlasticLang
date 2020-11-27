namespace PlasticLang.Ast
{
    public record Symbol(string Value) : Syntax
    {
        public override string ToString() => Value;
    }
}