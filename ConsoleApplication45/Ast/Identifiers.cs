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

        //evaluating a list of identifiers means just using the first one
        public object Eval(PlasticContext context)
        {
            var first = Values.FirstOrDefault();
            if (first == null)
                return null;

            return context[first.Name];
        }
    }
}