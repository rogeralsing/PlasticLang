using System.Linq;
using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public record ListValue : Syntax
    {
        public ListValue(params Syntax[] elements)
        {
            Elements = elements;
        }

        public Syntax[] Elements { get; }

        public Syntax[] Args => Elements.Skip(1).ToArray();

        public Syntax Head => Elements.First();

        public static ListValue CallFunction(string name, params Syntax[] args)
        {
            return new(new Symbol(name).Union(args));
        }

        public ValueTask<object> Eval(PlasticContext context)
        {
            return context.Invoke(Head, Args);
        }

        public override string ToString()
        {
            return $"{Head}({string.Join(",", Args.Select(a => a.ToString()))})";
        }
    }
}