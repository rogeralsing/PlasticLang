using System;
using System.Threading.Tasks;
using PlasticLang.Ast;
using PlasticLang.Contexts;

namespace PlasticLang.Visitors
{
    public static class Evaluator
    {
        public static async ValueTask<T> Eval<T>(this Syntax syn, PlasticContext context)
        {
            var res = await syn.Eval(context);
            return (T) res;
        }
        public static ValueTask<object> Eval(this Syntax syn, PlasticContext context) =>
            syn switch
            {
                NumberLiteral numberLiteral => EvalNumberLiteral(context, numberLiteral),
                StringLiteral str           => EvalStringLiteral(context, str),
                Symbol symbol               => EvalSymbol(context, symbol),
                ListValue listValue         => EvalListValue(context, listValue),
                ArrayValue arrayValue       => EvalArray(context, arrayValue),
                Statements statements       => EvalStatements(context, statements),
                TupleValue tupleValue       => EvalTupleValue(context, tupleValue),
                _                           => throw new ArgumentOutOfRangeException(nameof(syn))
            };

        private static async ValueTask<object> EvalTupleValue(PlasticContext context, TupleValue tupleValue)
        {
            var items = new object[tupleValue.Items.Length];
            for (var i = 0; i < tupleValue.Items.Length; i++)
            {
                var v = await Eval(tupleValue.Items[i], context);
                items[i] = v;
            }

            var res = new TupleInstance(items);
            return res;
        }

        private static async ValueTask<object> EvalStatements(PlasticContext context, Statements statements)
        {
            ValueTask<object> result = default;
            foreach (var statement in statements.Elements) result = Eval(statement, context);
            return await result;
        }

        private static async ValueTask<object> EvalNumberLiteral(PlasticContext context, NumberLiteral numberLiteral)
        {
            return await context.Number(numberLiteral);
        }

        private static async ValueTask<object> EvalListValue(PlasticContext context, ListValue listValue)
        {
            return await context.Invoke(listValue.Head, listValue.Rest);
        }

        private static ValueTask<object> EvalSymbol(PlasticContext context, Symbol symbol)
        {
            return ValueTask.FromResult(context[symbol.Value]);
        }

        private static ValueTask<object> EvalStringLiteral(PlasticContext context, StringLiteral str)
        {
            return context.QuotedString(str);
        }

        private static async ValueTask<object> EvalArray(PlasticContext context, ArrayValue arrayValue)
        {
            var items = new object[arrayValue.Items.Length];
            for (var i = 0; i < arrayValue.Items.Length; i++) items[i] = await arrayValue.Items[i].Eval(context);
            return items;
        }
    }
}