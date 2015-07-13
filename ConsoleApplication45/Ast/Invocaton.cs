using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace PlasticLangLabb1.Ast
{
    public class Invocaton : IExpression
    {

        public Invocaton(IExpression head, IEnumerable<IExpression> args)
        {

            Head = head;
            Args = args.ToArray();
        }

        public IExpression[] Args { get; set; }
        public IExpression Head { get; set; }
    }
}