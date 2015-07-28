using System.Linq;

namespace PlasticLang.Ast
{
    public class Invocation : IExpression
    {
        public Invocation(IExpression head, params IExpression[] args)
        {
            Head = head;
            Args = args;
        }

        public Invocation(IExpression head, TupleValue args, IExpression body = null)
        {
            Head = head;
            var tmp = args != null ? args.Items : new IExpression[0];

            if (body != null)
            {
                var oneBody = Enumerable.Repeat(body, 1).ToArray();
                tmp = tmp.Union(oneBody).ToArray();
            }
            Args = tmp.ToArray();
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