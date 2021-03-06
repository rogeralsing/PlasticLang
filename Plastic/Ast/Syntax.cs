﻿using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace PlasticLang.Ast
{
    public abstract record Syntax;

    public sealed record StringLiteral(string Value) : Syntax;


    public sealed record Symbol : Syntax
    {
        private static int _idCounter;
        private static readonly ConcurrentDictionary<string, int> id = new();
        public static readonly Symbol Last = new("last");

        public Symbol(string identity)
        {
            Identity = identity;
            IdNum = id.GetOrAdd(identity, i => Interlocked.Increment(ref _idCounter));
        }

        public string Identity { get; }
        public int IdNum { get; }
    }

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