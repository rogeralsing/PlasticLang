namespace PlasticLangLabb1.Ast
{
    public class Invocaton : IExpression
    {
        public Invocaton(IExpression exp, IExpression args, Statements body)
        {
            Expression = exp;
            Args = args;
            Body = body;
        }

        public IExpression Expression { get; set; }
        public Statements Body { get; set; }
        public IExpression Args { get; set; }
    }
}