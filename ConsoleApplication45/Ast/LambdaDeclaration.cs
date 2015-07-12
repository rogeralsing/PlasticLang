using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class LambdaDeclaration : IExpression
    {
        public LambdaDeclaration(IEnumerable<Identifier> args, IExpression body)
        {
            Args = args.ToArray();

            Body = body;
        }

        public IExpression Body { get; set; }
        public Identifier[] Args { get; set; }
    }
}