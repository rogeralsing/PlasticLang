namespace PlasticLang.Ast
{
    public class Assignment : IExpression
    {
        public Assignment(IExpression assignee, IExpression expression)
        {
            Assignee = assignee;
            Expression = expression;
        }

        public IExpression Assignee { get; set; }
        public IExpression Expression { get; set; }

        public object Eval(PlasticContext context)
        {
            var value = Expression.Eval(context);
            var dot = Assignee as BinaryExpression;
            var assignee = Assignee as Identifier;

            if (assignee != null)
            {
                context[assignee.Value] = value;
            }

            if (dot != null)
            {
                var obj = dot.Left.Eval(context) as PlasticObject;
                var memberId = dot.Right as Identifier;
                obj[memberId.Value] = value;
            }

            return value;
        }

        private void AssignToIdentifier(Identifier assignee, PlasticContext context, object value)
        {
            context[assignee.Value] = value;
        }
    }
}