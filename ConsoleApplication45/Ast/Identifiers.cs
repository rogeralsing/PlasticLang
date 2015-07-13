using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class Identifiers : IExpression
    {
        public Identifiers(IEnumerable<Identifier> ids)
        {

            Values = ids.ToArray();
        }

        public Identifier[] Values { get; private set; }
    }
}