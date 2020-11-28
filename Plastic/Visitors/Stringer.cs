using System;
using System.Globalization;
using System.Linq;
using PlasticLang.Ast;
using PlasticLang.Contexts;

namespace PlasticLang.Visitors
{
    public static class Stringer
    {
        public static string ToString(this Syntax syn, PlasticContext context)
        {
            return syn switch
                   {
                       ArrayValue arrayValue       => StringArrayValue(arrayValue),
                       StringLiteral str           => StringStringLiteral(str),
                       Symbol symbol               => StringSymbol(symbol),
                       ListValue listValue         => StringListValue(listValue),
                       NumberLiteral numberLiteral => StringNumberLiteral(numberLiteral),
                       Statements statements       => StringStatements(statements),
                       TupleValue tupleValue       => StringTupleValue(tupleValue),
                       _                           => throw new ArgumentOutOfRangeException(nameof(syn))
                   };
        }

        private static string StringTupleValue(TupleValue tupleValue)
        {
            return $"({string.Join(",", tupleValue.Items.Select(i => i.ToString()))})";
        }

        private static string StringStatements(Statements statements)
        {
            return "{" + string.Join(Environment.NewLine, statements.Elements.Select(s => s.ToString())) + "}";
        }

        private static string StringNumberLiteral(NumberLiteral numberLiteral)
        {
            return numberLiteral.Value.ToString(CultureInfo.InvariantCulture);
        }

        private static string StringListValue(ListValue listValue)
        {
            return $"{listValue.Head}({string.Join(",", listValue.Rest.Select(a => a.ToString()))})";
        }

        private static string StringSymbol(Symbol symbol)
        {
            return symbol.Identity;
        }

        private static string StringStringLiteral(StringLiteral str)
        {
            return $"\"{str.Value}\"";
        }

        private static string StringArrayValue(ArrayValue arrayValue)
        {
            return $"[{string.Join(",", arrayValue.Items.Select(i => i.ToString()))}]";
        }
    }
}