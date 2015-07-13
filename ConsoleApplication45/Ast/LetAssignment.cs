﻿using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class LetAssignment : IExpression
    {

        public LetAssignment(IEnumerable<Identifier> cells, IExpression expression)
        {
            this.Cells = cells.ToArray();
            this.Expression = expression;
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