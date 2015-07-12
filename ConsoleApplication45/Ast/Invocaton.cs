using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class Invocaton : IExpression
    {
        private Identifiers identifiers;
        private IEnumerable<IExpression> enumerable;
        private Statements statements;

        public Invocaton(IEnumerable<IExpression> args, Statements body)
        {
            Args = args.ToArray();
            Body = body;
        }

        public Invocaton(Identifiers identifiers, IEnumerable<IExpression> enumerable, Statements statements)
        {
            // TODO: Complete member initialization
            this.identifiers = identifiers;
            this.enumerable = enumerable;
            this.statements = statements;
        }

        public Statements Body { get; set; }
        public IExpression[] Args { get; set; }
    }
}