using PlasticLang.Ast;

namespace PlasticLang.Contexts
{
    public abstract class PlasticContext
    {
        protected PlasticContext(PlasticContextImpl? parent)
        {
            Parent = parent;
        }

        protected PlasticContextImpl? Parent { get; }

        public abstract object? this[Symbol symbol] { get; set; }

        public abstract object Number(NumberLiteral numberLiteral);
        public abstract object QuotedString(StringLiteral stringLiteral);
        public abstract object? Invoke(Syntax head, Syntax[] args);
    }
}