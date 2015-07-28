using System.Linq;

namespace PlasticLang.Ast
{
    public class ListValue : IExpression
    {
        public static ListValue CallFunction(string name, params IExpression[] args)
        {
            return new ListValue(new Symbol(name).Union(args));
        }

        public ListValue(params IExpression[] elements)
        {
            Elements = elements;
        }

        public IExpression[] Elements { get; private set; }

        public IExpression[] Args
        {
            get { return Elements.Skip(1).ToArray(); }
        }

        public IExpression Head
        {
            get { return Elements.First(); }
        }

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