using System.Linq;

namespace PlasticLang.Ast
{
    public record ListValue : Syntax
    {
        public ListValue(params Syntax[] elements)
        {
            Elements = elements;
            Head = Elements.First();
            Rest = Elements.Skip(1).ToArray();
        }

        private Syntax[] Elements { get; }

        public Syntax[] Rest { get; } 

        public Syntax Head { get; }

        public static ListValue CallFunction(string name, params Syntax[] args) => new Symbol(name).Union(args).ToListValue();

        public override string ToString() => $"{Head}({string.Join(",", Rest.Select(a => a.ToString()))})";
    }
}