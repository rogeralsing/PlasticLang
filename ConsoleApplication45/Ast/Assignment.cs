using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class Assignment : IExpression
    {
        public Assignment(IEnumerable<Identifier> cells, IExpression expression)
        {
            Cells = cells.ToArray();
            Expression = expression;
        }

        public Identifier[] Cells { get; set; }
        public IExpression Expression { get; set; }

        public object Eval(PlasticContext context)
        {
            var value = Expression.Eval(context);
            foreach (var cell in Cells)
            {
                context[cell.Name] = value;
            }
            return value;
        }
    }
}