using System;
using System.Globalization;
using System.Linq;
using PlasticLang.Ast;
using PlasticLang.Contexts;

namespace PlasticLang.Visitors
{
    public static class Stringer
    {
        public static string ToString(this Syntax syn, PlasticContext context) =>
            syn switch
            {
                ArrayValue arrayValue       => $"[{string.Join(",", arrayValue.Items.Select(i => i.ToString()))}]",
                StringLiteral str           => $"\"{str.Value}\"",
                Symbol symbol               => symbol.Value,
                ListValue listValue         => $"{listValue.Head}({string.Join(",", listValue.Rest.Select(a => a.ToString()))})",
                NumberLiteral numberLiteral => numberLiteral.Value.ToString(CultureInfo.InvariantCulture),
                Statements statements       => "{" + string.Join(Environment.NewLine, statements.Elements.Select(s => s.ToString())) + "}",
                TupleValue tupleValue       => $"({string.Join(",", tupleValue.Items.Select(i => i.ToString()))})",
                _                           => throw new ArgumentOutOfRangeException(nameof(syn))
            };
    }
}