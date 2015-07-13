using System.Collections.Generic;
using System.Linq;

namespace PlasticLangLabb1.Ast
{
    public class MacroDeclaration : IExpression
    {
        public MacroDeclaration(IEnumerable<Identifier> args, IExpression body)
        {
            Args = (args ?? new Identifier[0]).ToArray();

            Body = body;
        }

        public IExpression Body { get; set; }
        public Identifier[] Args { get; set; }

        public object Eval(PlasticContext context)
        {
            PlasticMacro op = (callingContext,args) =>
            {
                //create context for this invocation
                var ctx = new PlasticContext(callingContext);
                int i = 0;
                foreach (var arg in Args)
                {
                    //copy args from caller to this context
                    ctx[arg.Name] = args[i];
                    i++;
                }

                var res = Body.Eval(callingContext);
                return res;
            };
            return op;
        }
    }
}
