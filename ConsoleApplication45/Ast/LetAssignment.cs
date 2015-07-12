namespace PlasticLangLabb1.Ast
{
    public class LetAssignment : IExpression
    {
        public LetAssignment(Identifier cell, IExpression expression)
        {
            Cell = cell;
            Expression = expression;
        }

        public Identifier Cell { get; set; }

        public IExpression Expression { get; set; }
    }
}
