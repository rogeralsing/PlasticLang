using System.Linq;
using System.Security.Policy;

namespace PlasticLang.Ast
{
    public class Invocation : IExpression
    {
        public static Invocation CallFunction(string name, params IExpression[] args)
        {
            return new Invocation(new Symbol(name), args);
        }

        public Invocation(IExpression head, params IExpression[] args)
        {
            Head = head;
            Args = args;
        }

        public IExpression[] Args { get; set; }
        public IExpression Head { get; set; }

        public object Eval(PlasticContext context)
        {
            return context.Invoke(Head, Args);
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", Head, string.Join(",", Args.Select(a => a.ToString())));
        }
    }
}