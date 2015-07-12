using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
