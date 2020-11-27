using System;
using System.Collections.Generic;
using System.Linq;

namespace PlasticLang.Ast
{
    public record Statements(IEnumerable<Syntax> Elements) : Syntax
    {
        public override string ToString()
        {
            return "{" + string.Join(Environment.NewLine, Elements.Select(s => s.ToString())) + "}";
        }
    }
}