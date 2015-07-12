using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class Identifiers : IExpression
    {
        public Identifiers(IEnumerable<IExpression> ids)
        {
            Values = ids.Cast<Identifier>().ToArray();
        }

        public Identifier[] Values { get; private set; }
    }
}