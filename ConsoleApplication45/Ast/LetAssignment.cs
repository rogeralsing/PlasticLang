using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class LetAssignment : IExpression
    {
        public LetAssignment(IEnumerable<Identifier> cells, IExpression expression)
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
                context.Declare(cell.Name, value);
            }
            return value;
        }

        public override string ToString()
        {
            return string.Format("let {0} = {1}", string.Join(",", Cells.Select(i => i.Name)), Expression);
        }
    }
}