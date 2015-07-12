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
        public LambdaDeclaration(Sprache.IOption<IEnumerable<Identifier>> args, IExpression body)
        {
            Args = args.IsDefined ? args.Get().ToArray() : new Identifier[0];

            Body = body;
        }

        public IExpression Body { get; set; }
        public Identifier[] Args { get; set; }
    }
}
