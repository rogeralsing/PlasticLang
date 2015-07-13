using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PlasticLangLabb1.Ast;
using Sprache;

namespace PlasticLangLabb1
{
    public class PlasticParser
    {
        public static Parser<BinaryOperator> BinOp(string op, BinaryOperator node)
        {
            return Parse.String(op).Token().Return(node);
        }

        public static readonly Parser<Identifier> Identifier =
            (from first in Parse.Letter.Once().Text()
                from rest in Parse.LetterOrDigit.Many().Text()
                select new Identifier(first + rest))
                .Token();

        public static readonly Parser<Identifiers> Identifiers =
            from ids in Parse.Ref(() => Identifier).AtLeastOnce()
            select new Identifiers(ids);

        public static readonly Parser<Number> Number =
            (from numb in Parse.DecimalInvariant
                select new Number(numb)).Token();

        public static readonly Parser<QuotedString> QuotedString = MakeString('"').Or(MakeString('\''));

        private static Parser<QuotedString> MakeString(char quote)
        {
            return (from str in Parse.CharExcept(quote).Many().Contained(Parse.Char(quote), Parse.Char(quote))
                select new QuotedString(new string(str.ToArray()))).Token();
        }

        public static readonly Parser<IExpression> Literal =
            Identifiers.Select(x => x as IExpression)
                .Or(Number)
                .Or(QuotedString);

        public static readonly Parser<IExpression> Value =
            Parse.Ref(() => TupleValue)
                .Or(Parse.Ref(() => Literal));

        public static readonly Parser<BinaryOperator> MultiplyOperator = BinOp("*", new MultiplyBinary());
        public static readonly Parser<BinaryOperator> DivideOperator = BinOp("/", new DivideBinary());
        public static readonly Parser<BinaryOperator> AddOperator = BinOp("+", new AddBinary());
        public static readonly Parser<BinaryOperator> SubtractOperator = BinOp("-", new SubtractBnary());
        public static readonly Parser<BinaryOperator> EqualsOperator = BinOp("==", new EqualsBinary());

        public static readonly Parser<IExpression> InnerTerm = Parse.ChainOperator(AddOperator.Or(SubtractOperator),
            Parse.Ref(() => InvocationOrValue), (o, l, r) => new BinaryExpression(l, o, r));

        public static readonly Parser<IExpression> Term = Parse.ChainOperator(MultiplyOperator.Or(DivideOperator),
            InnerTerm, (o, l, r) => new BinaryExpression(l, o, r));

        public static readonly Parser<IExpression> Compare = Parse.ChainOperator(EqualsOperator,
            Term, (o, l, r) => new BinaryExpression(l, o, r));

        public static readonly Parser<IExpression> Assign =
            from x in Parse.String("let").Token()
            from cells in Identifier.DelimitedBy(Parse.Char(',').Token())
            from assignOp in Parse.String("=").Token()
            from expression in Parse.Ref(() => Expression)
            select new LetAssignment(cells, expression);

        public static readonly Parser<IExpression> Expression =
            Parse.Ref(() => LambdaDeclaration)
                .Or(Parse.Ref(() => Assign))
                .Or(Parse.Ref(() => Compare))
                .Or(Parse.Ref(() => Body));

        public static readonly Parser<IExpression> TerminatedStatement =
            from exp in Parse.Ref(() => Expression)
            from _ in Parse.Char(';').Token()
            select exp;

        public static readonly Parser<IExpression> Statement =
            Parse.Ref(() => TerminatedStatement);

        //    .Or(Parse.Ref(() => InvocationStatement));               

        public static readonly Parser<Statements> Statements =
            from statements in Statement.Many()
            select new Statements(statements);

        public static readonly Parser<Statements> Body =
            from lbrace in Parse.Char('{').Token()
            from statements in Statement.Many()
            from rbrace in Parse.Char('}').Token()
            select new Statements(statements);

        public static readonly Parser<IExpression> LambdaBody = Parse.Ref(() => Expression);

        public static readonly Parser<IEnumerable<Identifier>> LambdaArgs =
            Identifier
                .DelimitedBy(Parse.Char(',').Token())
                .Optional()
                .Contained(Parse.Char('(').Token(), Parse.Char(')').Token())
                .Select(o => o.GetOrDefault())
                .Or(Identifier.Once());

        public static readonly Parser<IExpression> LambdaDeclaration =
            from args in LambdaArgs
            from arrow in Parse.String("=>").Token()
            from body in Parse.Ref(() => LambdaBody)
            select new FunctionDeclaration(args, body);

        public static readonly Parser<IExpression> TupleValue =
            Parse.Ref(() => Expression)
                .DelimitedBy(Parse.Char(',').Token())
                .Optional()
                .Contained(Parse.Char('(').Token(), Parse.Char(')').Token())
                .Select(o => new TupleValue(o.IsDefined ? o.Get() : Enumerable.Empty<IExpression>()));

        public static readonly Parser<IExpression> TupleOrBody = TupleValue.Or(Parse.Ref(() => Body));

        public static readonly Parser<IExpression> InvocationOrValue =
            from head in Parse.Ref(() => Value)
            from args in TupleOrBody.Many()
            select args.Any()
                ? new Invocaton(head, args)
                : head;

        public static readonly Parser<IExpression> InvocationStatement =
            from head in InvocationOrValue
            from b in Parse.Ref(() => Body)
            select new Invocaton(head, new[] {b});
    }
}