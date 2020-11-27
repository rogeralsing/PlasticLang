using System.Linq;

namespace PlasticLang.Ast
{
    public record ListValue : Syntax
    {
        public ListValue(params Syntax[] elements)
        {
            Elements = elements;
            Head = Elements.First();
        }

        public Syntax[] Elements { get; }

        public Syntax[] Args => Elements.Skip(1).ToArray();

        public Syntax Head { get; }

        public static ListValue CallFunction(string name, params Syntax[] args)
        {
            return new(new Symbol(name).Union(args));
        }

        public override string ToString()
        {
            return $"{Head}({string.Join(",", Args.Select(a => a.ToString()))})";
        }
    }
}