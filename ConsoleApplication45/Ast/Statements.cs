using System.Collections.Generic;
using PlasticLang.Ast;

namespace PlasticLang.Ast
{
    public class Statements : IExpression
    {
        private readonly IEnumerable<IExpression> _statements;

        public Statements(IEnumerable<IExpression> statements)
        {
            _statements = statements;
        }

        public object Eval(PlasticContext context)
        {
            object result = null;
            foreach (var statement in _statements)
            {
                result = statement.Eval(context);
            }
            return result;
        }
    }
}