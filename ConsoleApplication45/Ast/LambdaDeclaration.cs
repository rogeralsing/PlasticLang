using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class LambdaDeclaration : IExpression
    {
        public LambdaDeclaration(IEnumerable<Identifier> args, IExpression body)
        {
            Args = (args ?? new Identifier[0]).ToArray();

            Body = body;
        }

        public IExpression Body { get; set; }
        public Identifier[] Args { get; set; }

        public object Eval(PlasticContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}