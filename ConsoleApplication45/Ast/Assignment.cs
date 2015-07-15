using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
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
            var assignee =Assignee as Identifier;
            
            if (assignee != null)
            {
                context[assignee.Name] = value;        
            }

            if (dot != null)
            {
                var obj = dot.Left.Eval(context) as PlasticObject;
                var memberId = dot.Right as Identifier;
                obj[memberId.Name] = value;
            }

            //foreach (var cell in Cells)
            //{
            //    context[cell.Name] = value;
            //}
            return value;
        }

        private void AssignToIdentifier(Identifier assignee, PlasticContext context, object value)
        {
            context[assignee.Name] = value;            
        }
    }
}