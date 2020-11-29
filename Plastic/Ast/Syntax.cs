using System.Collections.Generic;
using System.Linq;
using PlasticLang.Contexts;

namespace PlasticLang.Ast
{
    public abstract record Syntax;

    public sealed record StringLiteral(string Value) : Syntax;

    public sealed record Symbol(string Identity) : Syntax;
    public sealed record Statements(Syntax[] Elements) : Syntax;

    public sealed record ArrayValue(Syntax[] Items) : Syntax;

    public sealed record NumberLiteral(decimal Value) : Syntax
    {
        public static readonly NumberLiteral One = new(1m);
    }

    public sealed record TupleValue(Syntax[] Items) : Syntax;

    public sealed record ListValue : Syntax
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

        public static ListValue CallFunction(string name, params Syntax[] args)
        {
            return new Symbol(name).Union(args).ToListValue();
        }
    }
}