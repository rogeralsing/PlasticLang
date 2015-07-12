using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasticLangLabb1.Ast
{
    public class Statements : IExpression
    {
        private IEnumerable<IExpression> _statements;

        public Statements(IEnumerable<IExpression> _statements)
        {
            // TODO: Complete member initialization
            this._statements = _statements;
        }
    }
}
