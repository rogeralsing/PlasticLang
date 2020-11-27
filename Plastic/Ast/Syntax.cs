using System.Collections.Generic;
using System.Linq;

namespace PlasticLang.Ast
{
    public abstract record Syntax;
    public record StringLiteral(string Value) : Syntax;
    public record Symbol(string Value) : Syntax;
    public record Statements(IEnumerable<Syntax> Elements) : Syntax;
    public record ArrayValue(Syntax[] Items) : Syntax;
    
    public record NumberLiteral(decimal Value) : Syntax
    {
        public static readonly NumberLiteral One = new(1m);
    }
    
    public record TupleValue(Syntax[] Items) : Syntax;
    
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

    }
}