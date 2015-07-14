﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class TupleValue : IExpression
    {
        public TupleValue(IEnumerable<IExpression> items)
        {
            Items = items.ToArray();
        }

        public IExpression[] Items { get; set; }

        public object Eval(PlasticContext context)
        {
            if (Items.Length == 1)
                return Items[0].Eval(context);

            return Items.Select(i => i.Eval(context)).ToArray();
        }

        public override string ToString()
        {
            return string.Format("({0})", string.Join(",", Items.Select(i => i.ToString())));
        }
    }
}