using System;
using System.Collections.Generic;
using System.Linq;
using PlasticLang.Ast;
using Sprache;

namespace PlasticLang
{
    public class PlasticParser
    {
        public static readonly Parser<string> MultiplyOperator = BinOps("*", "_mul");
        public static readonly Parser<string> DivideOperator = BinOps("/", "_div");
        public static readonly Parser<string> AddOperator = BinOps("+", "_add");
        public static readonly Parser<string> SubtractOperator = BinOps("-", "_sub");
        public static readonly Parser<string> EqualsOperator = BinOps("==", "_eq");
        public static readonly Parser<string> NotEqualsOperator = BinOps("!=", "_neq");
        public static readonly Parser<string> GreaterThanOperator = BinOps(">", "_gt");
        public static readonly Parser<string> GreateerOrEqualOperator = BinOps(">=", "_gteq");
        public static readonly Parser<string> LessThanOperator = BinOps("<", "_lt");
        public static readonly Parser<string> LessOrEqualOperator = BinOps("<=", "_lteq");

        public static readonly Parser<string> BooleanOr = BinOps("||", "_bor");
        public static readonly Parser<string> BooleanAnd = BinOps("&&", "_band");

        public static readonly Parser<string> DotOperator = BinOps(".", "_dot");


        public static Parser<string> BinOps(string op, string name)
        {
            return Parse.String(op).PlasticToken().Return(name);
        }

        private static Parser<StringLiteral> MakeString(char quote)
        {
            return (from str in Parse.CharExcept(quote).Many().Contained(Parse.Char(quote), Parse.Char(quote))
                select new StringLiteral(new string(str.ToArray()))).PlasticToken();
        }

        private static IExpression CreateInvocations(IExpression head, TupleValue[] argsList, Statements body)
        {
            var current = head;
            for (var i = 0; i < argsList.Length; i++)
            {
                var tuple = argsList[i];
                var args = tuple.Items;
                if (i == argsList.Length - 1 && body != null)
                {
                    args = args.Union(Enumerable.Repeat(body, 1)).ToArray();
                }
                current = new ListValue(current.Union(args));
            }
            if (argsList.Length == 0 && body != null)
            {
                current = new ListValue(current, body);
            }
            return current;
        }

        public static Parser<IOption<char>> Separator = Parse.Char(',').Or(Parse.Char(';')).Optional().PlasticToken();

        public static readonly Parser<Symbol> Symbol =
            (from first in Parse.Letter.Or(Parse.Chars('_', '@')).Once().Text()
                from rest in Parse.LetterOrDigit.Or(Parse.Chars('!', '?', '_')).Many().Text()
                select new Symbol(first + rest))
                .PlasticToken();

        public static readonly Parser<NumberLiteral> Number =
            (from numb in Parse.DecimalInvariant
                select new NumberLiteral(numb)).PlasticToken();

        public static readonly Parser<StringLiteral> QuotedString = MakeString('"').Or(MakeString('\''));

        public static readonly Parser<IExpression> Literal =
            Symbol.Select(x => x as IExpression)
                .Or(Number)
                .Or(QuotedString);

        public static readonly Parser<IExpression> IdentifierInc =
            from symbol in Symbol
            from plusplus in Parse.String("++").PlasticToken()
            select ListValue.CallFunction("assign", symbol, ListValue.CallFunction("_add", symbol, NumberLiteral.One));

        public static readonly Parser<IExpression> IdentifierDec =
            from symbol in Symbol
            from plusplus in Parse.String("--").PlasticToken()
            select ListValue.CallFunction("assign", symbol, ListValue.CallFunction("_sub", symbol, NumberLiteral.One));

        public static readonly Parser<IExpression> Value =
            Parse.Ref(() => TupleValue)
                .Or(Parse.Ref(() => ArrayValue))
                .Or(Parse.Ref(() => IdentifierInc))
                .Or(Parse.Ref(() => IdentifierDec))
                .Or(Parse.Ref(() => Literal))
                .Or(Parse.Ref(() => Body));

        public static readonly Parser<IExpression> DotTerm = Parse.ChainOperator(DotOperator,
            Parse.Ref(() => InvocationOrValue), (o, l, r) =>  ListValue.CallFunction("_dot",l,r));

        public static readonly Parser<IExpression> InnerTerm = Parse.ChainOperator(AddOperator.Or(SubtractOperator),
            Parse.Ref(() => DotTerm), (o, l, r) => ListValue.CallFunction(o, l, r));

        public static readonly Parser<IExpression> Term = Parse.ChainOperator(MultiplyOperator.Or(DivideOperator),
            InnerTerm, (o, l, r) => ListValue.CallFunction(o, l, r));

        public static readonly Parser<IExpression> Compare =
            Parse.ChainOperator(
                EqualsOperator.Or(NotEqualsOperator)
                    .Or(GreateerOrEqualOperator)
                    .Or(GreaterThanOperator)
                    .Or(LessOrEqualOperator)
                    .Or(LessThanOperator),
                Term, (o, l, r) => ListValue.CallFunction(o, l, r));


        public static readonly Parser<IExpression> BooleanLogic =
            Parse.ChainOperator(
                BooleanOr.Or(BooleanAnd),
                Compare, (o, l, r) => ListValue.CallFunction(o, l, r));

        

        public static readonly Parser<IExpression> AssignTerm = Parse.ChainOperator(Parse.Char('=').PlasticToken(),
            Parse.Ref(() => BooleanLogic), (o, l, r) =>
                ListValue.CallFunction("assign", l, r));

        public static readonly Parser<IExpression> LetAssign =
            from symbol in Symbol
            from assignOp in Parse.String(":=").PlasticToken()
            from expression in Parse.Ref(() => Expression)
            select ListValue.CallFunction("def", symbol, expression);

        public static readonly Parser<IExpression> Expression =
            Parse.Ref(() => LambdaDeclaration)
                .Or(Parse.Ref(() => LetAssign))
                .Or(Parse.Ref(() => AssignTerm));

        public static readonly Parser<IExpression> TerminatedStatement =
            from exp in Parse.Ref(() => Expression)
            from _ in Separator.Optional().PlasticToken()
            select exp;

        public static readonly Parser<IExpression> Statement =
            Parse.Ref(() => TerminatedStatement);

        public static readonly Parser<Statements> Statements =
            from statements in Statement.Many()
            select new Statements(statements);

        public static readonly Parser<Statements> Body =
            from lbrace in Parse.Char('{').PlasticToken()
            from statements in Statements
            from rbrace in Parse.Char('}').PlasticToken()
            select statements;

        public static readonly Parser<IExpression> LambdaBody = Parse.Ref(() => Expression);

        public static readonly Parser<IEnumerable<IExpression>> LambdaArgs =
            Expression
                .DelimitedBy(Separator)
                .Optional()
                .Contained(Parse.Char('(').PlasticToken(), Parse.Char(')').PlasticToken())
                .Select(o => o.GetOrDefault())
                .Or(Symbol.Once());

        public static readonly Parser<IExpression> LambdaDeclaration =
            from args in LambdaArgs
            from arrow in Parse.String("=>").PlasticToken()
            from body in Parse.Ref(() => LambdaBody).Once()
            select ListValue.CallFunction("func", args.Union(body).ToArray());

        public static readonly Parser<TupleValue> TupleValue =
            Parse.Ref(() => Expression)
                .DelimitedBy(Separator)
                .Optional()
                .Contained(Parse.Char('(').PlasticToken(), Parse.Char(')').PlasticToken())
                .Select(o => new TupleValue(o.IsDefined ? o.Get() : Enumerable.Empty<IExpression>()));

        public static readonly Parser<IExpression> ArrayValue =
            Parse.Ref(() => Expression)
                .DelimitedBy(Separator)
                .Optional()
                .Contained(Parse.Char('[').PlasticToken(), Parse.Char(']').PlasticToken())
                .Select(o => new ArrayValue(o.IsDefined ? o.Get() : Enumerable.Empty<IExpression>()));

        public static readonly Parser<IExpression> InvocationOrValue =
            from head in Parse.Ref(() => Value)
            from args in TupleValue.Many()
            from body in Body.Optional()
            select CreateInvocations(head, args.ToArray(), body.GetOrDefault());
    }

    public static class ParseExtensions
    {
        public static Parser<T> PlasticToken<T>(this Parser<T> self)
        {
            return from _ in Ws
                from item in self
                from __ in Ws
                select item;
        }

        private static readonly CommentParser Comments = new CommentParser("//", "/*", "*/", Environment.NewLine);

        private static readonly Parser<string> Ws =
            from _ in Parse.WhiteSpace.Many()
            from c in Comments.AnyComment.Optional()
            from __ in Parse.WhiteSpace.Many()
            select "";
    }
}