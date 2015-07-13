namespace PlasticLangLabb1.Ast
{
    public class IdentifierInc : IExpression
    {
        private readonly Identifier _identifier;

        public IdentifierInc(Identifier identifier)
        {
            _identifier = identifier;
        }

        public object Eval(PlasticContext context)
        {
            dynamic value = _identifier.Eval(context);
            dynamic newValue = value + 1;
            context[_identifier.Name] = newValue;
            return newValue;
        }
    }

    public class IdentifierDec : IExpression
    {
        private readonly Identifier _identifier;

        public IdentifierDec(Identifier identifier)
        {
            _identifier = identifier;
        }

        public object Eval(PlasticContext context)
        {
            dynamic value = _identifier.Eval(context);
            dynamic newValue = value - 1;
            context[_identifier.Name] = newValue;
            return newValue;
        }
    }
}
