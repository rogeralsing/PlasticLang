using System;
using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class FunctionDeclaration : IExpression
    {
        public FunctionDeclaration(IEnumerable<Identifier> args, IExpression body)
        {
            Args = (args ?? new Identifier[0]).ToArray();

            Body = body;
        }

        public IExpression Body { get; set; }
        public Identifier[] Args { get; set; }

        public object Eval(PlasticContext context)
        {
            PlasticInterop op = args =>
            {
                //create context for this invocation
                var ctx = new PlasticContext(context);
                int i = 0;
                foreach (var arg in Args)
                {
                    //copy args from caller to this context
                    ctx[arg.Name] = args[i];
                }

                var res = Body.Eval(ctx);
                return res;
            };
            return op;
        }
    }
}