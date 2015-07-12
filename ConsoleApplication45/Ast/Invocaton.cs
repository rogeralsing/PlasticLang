using System.Collections.Generic;
using System.ComponentModel.Design;

namespace PlasticLangLabb1.Ast
{
    public class Invocaton : IExpression
    {
        public Invocaton(Identifiers identifiers, IExpression args, Statements body)
        {
            Identifiers = identifiers;
            Args = args;
            Body = body;
        }

        public Identifiers Identifiers { get; set; }

        public Statements Body { get; set; }

        public IExpression Args { get; set; }
    }
}