using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasticLangLabb1.Ast
{
    public class Invocations : IExpression
    {
        public Invocations(IEnumerable<Invocaton> invocations)
        {
            Values = invocations.ToArray();
        }

        public Invocaton[] Values { get; set; }
    }
}
