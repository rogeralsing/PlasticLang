using System.Linq;
using System.Threading.Tasks;

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

        public IExpression[] Elements { get; }

        public IExpression[] Args => Elements.Skip(1).ToArray();

        public IExpression Head => Elements.First();

        public Task<object> Eval(PlasticContext context)
        {
            return context.Invoke(Head, Args);
        }

        public override string ToString()
        {
            return $"{Head}({string.Join(",", Args.Select(a => a.ToString()))})";
        }
    }
}