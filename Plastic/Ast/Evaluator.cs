using System;
using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public static class Evaluator
    {
        public static ValueTask<object> Eval(this Syntax syn, PlasticContext context) =>
            syn switch
            {
                ArrayValue arrayValue       => arrayValue.Eval(context),
                StringLiteral str           => str.Eval(context),
                Symbol symbol               => symbol.Eval(context),
                ListValue listValue         => listValue.Eval(context),
                NumberLiteral numberLiteral => numberLiteral.Eval(context),
                Statements statements       => statements.Eval(context),
                TupleValue tupleValue       => tupleValue.Eval(context),
            };
    }
}