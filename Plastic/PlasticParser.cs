using System;
using System.Collections.Generic;
using System.Linq;
using PlasticLang.Ast;
using Sprache;

namespace PlasticLang
{
    public class PlasticParser
    {
        public static Parser<IOption<char>> Separator = Parse.Char(',').Or(Parse.Char(';')).Optional().PlasticToken();
 
        public static Parser<BinaryOperator> BinOp(string op, BinaryOperator node)
        {
            return Parse.String(op).PlasticToken().Return(node);
        }

        private static Parser<QuotedString> MakeString(char quote)
        {
            return (from str in Parse.CharExcept(quote).Many().Contained(Parse.Char(quote), Parse.Char(quote))
                select new QuotedString(new string(str.ToArray()))).PlasticToken();
        }

        private static IExpression CreateInvocations(IExpression head, TupleValue[] argsList, Statements body)
        {
            var current = head;
            for (var i = 0; i < argsList.Length; i++)
            {
                var args = argsList[i];
                current = new Invocation(current, args, i == argsList.Length - 1 ? body : null);
            }
            if (argsList.Length == 0 && body != null)
            {
                current = new Invocation(current, null, body);
            }
            return current;
        }

        public static readonly Parser<Identifier> Identifier =
            (from first in Parse.Letter.Once().Text()
                from rest in Parse.LetterOrDigit.Many().Text()
                select new Identifier(first + rest))
                .PlasticToken();

        public static readonly Parser<Number> Number =
            (from numb in Parse.DecimalInvariant
                select new Number(numb)).PlasticToken();

        public static readonly Parser<QuotedString> QuotedString = MakeString('"').Or(MakeString('\''));

        public static readonly Parser<IExpression> Literal =
            Identifier.Select(x => x as IExpression)
                .Or(Number)
                .Or(QuotedString);

        public static readonly Parser<IExpression> IdentifierInc =
            from identifier in Identifier
            from plusplus in Parse.String("++").PlasticToken()
            select new Invocation(new Identifier("def"), new TupleValue(new IExpression[] { identifier, new BinaryExpression(identifier, new AddBinary(), new Number("1")) }), null);

        public static readonly Parser<IExpression> IdentifierDec =
            from identifier in Identifier
            from plusplus in Parse.String("--").PlasticToken()
            select new Invocation(new Identifier("def"), new TupleValue(new IExpression[] { identifier, new BinaryExpression(identifier, new SubtractBnary(), new Number("1")) }), null);

        public static readonly Parser<IExpression> Value =
                    Parse.Ref(() => TupleValue)
                .Or(Parse.Ref(() => ArrayValue))
                .Or(Parse.Ref(() => IdentifierInc))
                .Or(Parse.Ref(() => IdentifierDec))
                .Or(Parse.Ref(() => Literal))
                .Or(Parse.Ref(() => Body));

        public static readonly Parser<BinaryOperator> MultiplyOperator = BinOp("*", new MultiplyBinary());
        public static readonly Parser<BinaryOperator> DivideOperator = BinOp("/", new DivideBinary());
        public static readonly Parser<BinaryOperator> AddOperator = BinOp("+", new AddBinary());
        public static readonly Parser<BinaryOperator> SubtractOperator = BinOp("-", new SubtractBnary());
        public static readonly Parser<BinaryOperator> EqualsOperator = BinOp("==", new EqualsBinary());
        public static readonly Parser<BinaryOperator> NotEqualsOperator = BinOp("!=", new NotEqualsBinary());
        public static readonly Parser<BinaryOperator> GreaterThanOperator = BinOp(">", new GreaterThanBinary());
        public static readonly Parser<BinaryOperator> GreateerOrEqualOperator = BinOp(">=", new GreaterOrEqualBinary());
        public static readonly Parser<BinaryOperator> LessThanOperator = BinOp("<", new LessThanBinary());
        public static readonly Parser<BinaryOperator> LessOrEqualOperator = BinOp("<=", new LessOrEqualBinary());
        public static readonly Parser<BinaryOperator> DotOperator = BinOp(".", new DotBinary());

        public static readonly Parser<IExpression> DotTerm = Parse.ChainOperator(DotOperator,
            Parse.Ref(() => InvocationOrValue), (o, l, r) => new BinaryExpression(l, o, r));

        public static readonly Parser<IExpression> InnerTerm = Parse.ChainOperator(AddOperator.Or(SubtractOperator),
            Parse.Ref(() => DotTerm), (o, l, r) => new BinaryExpression(l, o, r));

        public static readonly Parser<IExpression> Term = Parse.ChainOperator(MultiplyOperator.Or(DivideOperator),
            InnerTerm, (o, l, r) => new BinaryExpression(l, o, r));

        public static readonly Parser<IExpression> Compare =
            Parse.ChainOperator(
                EqualsOperator.Or(NotEqualsOperator)
                    .Or(GreateerOrEqualOperator)
                    .Or(GreaterThanOperator)
                    .Or(LessOrEqualOperator)
                    .Or(LessThanOperator),
                Term, (o, l, r) => new BinaryExpression(l, o, r));

        public static readonly Parser<IExpression> AssignTerm = Parse.ChainOperator(Parse.Char('=').PlasticToken(),
            Parse.Ref(() => Compare), (o, l, r) =>
                new Invocation(new Identifier("def"), new TupleValue(new[] {l,r}),null));

        public static readonly Parser<IExpression> LetAssign =
            from cells in Identifier.Once()
            from assignOp in Parse.String(":=").PlasticToken()
            from expression in Parse.Ref(() => Expression)
            select new LetAssignment(cells, expression);

        public static readonly Parser<IExpression> Expression =
            Parse.Ref(() => LambdaDeclaration)
                .Or(Parse.Ref(() => LetAssign))
                .Or(Parse.Ref(() => AssignTerm));
                //.Or(Parse.Ref(() => Body));

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
                .Or(Identifier.Once());

        public static readonly Parser<IExpression> LambdaDeclaration =
            from args in LambdaArgs
            from arrow in Parse.String("=>").PlasticToken()
            from body in Parse.Ref(() => LambdaBody)
            select new Invocation(new Identifier("func"), new TupleValue(args), body);

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

        public static readonly Parser<ArgsAndBody> ArgsAndBody =
            (from args in TupleValue
                from body in Body.Optional()
                select new ArgsAndBody(args, body.GetOrDefault()))
                .Or(
                    from body in Body
                    select new ArgsAndBody(null, body));

        public static readonly Parser<IExpression> InvocationOrValue =
            from head in Parse.Ref(() => Value)
            from args in TupleValue.Many()
            from body in Body.Optional()
            select CreateInvocations(head, args.ToArray(), body.GetOrDefault());
    }

    public static class ParseExtensions
    {
        private static readonly CommentParser Comments = new CommentParser("//", "/*", "*/",Environment.NewLine);
        private static readonly Parser<string> Ws =
            from _ in Parse.WhiteSpace.Many()
            from c in Comments.AnyComment.Optional()
            from __ in Parse.WhiteSpace.Many()
            select "";
        
        public static Parser<T> PlasticToken<T>(this Parser<T> self)
        {
            return from _ in Ws
                from item in self
                from __ in Ws
                select item;
        }
    }
}