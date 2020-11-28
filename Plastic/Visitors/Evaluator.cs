using System.Runtime.CompilerServices;
using PlasticLang.Ast;
using PlasticLang.Contexts;

namespace PlasticLang.Visitors
{
    public static class Evaluator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object Eval(this Syntax syn, PlasticContext context)
        {
            return syn switch
                   {
                       NumberLiteral numberLiteral => EvalNumberLiteral(context, numberLiteral),
                       StringLiteral str           => EvalStringLiteral(context, str),
                       Symbol symbol               => EvalSymbol(context, symbol),
                       ListValue listValue         => EvalListValue(context, listValue),
                       ArrayValue arrayValue       => EvalArray(context, arrayValue),
                       Statements statements       => EvalStatements(context, statements),
                       TupleValue tupleValue       => EvalTupleValue(context, tupleValue)
                   };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object EvalTupleValue(PlasticContext context, TupleValue tupleValue)
        {
            var items = new object[tupleValue.Items.Length];
            for (var i = 0; i < tupleValue.Items.Length; i++)
            {
                var v = Eval(tupleValue.Items[i], context);
                items[i] = v;
            }

            var res = new TupleInstance(items);
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object EvalStatements(PlasticContext context, Statements statements)
        {
            object result = default;
            foreach (var statement in statements.Elements) result = Eval(statement, context);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object EvalNumberLiteral(PlasticContext context, NumberLiteral numberLiteral)
        {
            return context.Number(numberLiteral);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object EvalListValue(PlasticContext context, ListValue listValue)
        {
            return context!.Invoke(listValue.Head!, listValue.Rest!)!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object EvalSymbol(PlasticContext context, Symbol symbol)
        {
            var v = context.GetSymbol(symbol);
            return v!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object EvalStringLiteral(PlasticContext context, StringLiteral str)
        {
            return context.QuotedString(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object EvalArray(PlasticContext context, ArrayValue arrayValue)
        {
            var items = new object[arrayValue.Items.Length];
            for (var i = 0; i < arrayValue.Items.Length; i++) items[i] = arrayValue.Items[i].Eval(context);
            return items;
        }
    }
}