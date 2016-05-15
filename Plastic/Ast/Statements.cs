using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public class Statements : IExpression
    {
        private readonly IEnumerable<IExpression> _statements;

        public Statements(IEnumerable<IExpression> statements)
        {
            _statements = statements;
        }

        public Task<object> Eval(PlasticContext context)
        {
            Task<object> result = null;
            foreach (var statement in _statements)
            {
                result = statement.Eval(context);
            }
            return result;
        }

        public override string ToString()
        {
            return "{" + string.Join(Environment.NewLine, _statements.Select(s => s.ToString())) + "}";
        }
    }
}