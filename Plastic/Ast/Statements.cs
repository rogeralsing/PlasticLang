using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public record Statements : Syntax
    {
        private readonly IEnumerable<Syntax> _statements;

        public Statements(IEnumerable<Syntax> statements)
        {
            _statements = statements;
        }

        public override ValueTask<object> Eval(PlasticContext context)
        {
            ValueTask<object> result = default;
            foreach (var statement in _statements) result = statement.Eval(context);
            return result;
        }

        public override string ToString()
        {
            return "{" + string.Join(Environment.NewLine, _statements.Select(s => s.ToString())) + "}";
        }
    }
}