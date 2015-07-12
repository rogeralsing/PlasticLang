using System.Linq;
using System.Security.Policy;
using PlasticLangLabb1.Ast;
using Sprache;

namespace PlasticLangLabb1
{
    internal class PlasticParser
    {
        public static readonly Parser<IExpression> Literal =
            PlasticPrimitives.Identifiers.Select(x => x as IExpression)
                .Or(PlasticPrimitives.Number)
                .Or(PlasticPrimitives.QuotedString);

        public static readonly Parser<IExpression> Value =
            Parse.Ref(() => ParenGroup)
                .Or(Parse.Ref(() => Literal));

        public static readonly Parser<BinaryOperator> MultiplyOperator = BinOp("*", new MultiplyBinary());
        public static readonly Parser<BinaryOperator> DivideOperator = BinOp("/", new DivideBinary());
        public static readonly Parser<BinaryOperator> AddOperator = BinOp("+", new AddBinary());
        public static readonly Parser<BinaryOperator> SubtractOperator = BinOp("-", new SubtractBnary());

        public static Parser<BinaryOperator> BinOp(string op, BinaryOperator node)
        {
            return Parse.String(op).Return(node);
        }

        public static readonly Parser<IExpression> InnerTerm = Parse.ChainOperator(AddOperator.Or(SubtractOperator),
            Value,
            (o, l, r) => new BinaryExpression(l, o, r));

        public static readonly Parser<IExpression> Term = Parse.ChainOperator(MultiplyOperator.Or(DivideOperator),
            InnerTerm,
            (o, l, r) => new BinaryExpression(l, o, r));

        public static readonly Parser<IExpression> Expression = Term;

        public static readonly Parser<IExpression> ParenGroup =
            from ws1 in Parse.WhiteSpace.Many()
            from lPar in Parse.Char('(').Once()
            from ws2 in Parse.WhiteSpace.Many()
            from exp in Parse.Ref(() => Parse.Ref(() => Expression))
            from ws3 in Parse.WhiteSpace.Many()
            from rPar in Parse.Char(')').Once()
            from ws4 in Parse.WhiteSpace.Many()
            select exp;

        public static readonly Parser<IExpression> Statement =
            from exp in Parse.Ref(() => Expression) 
            from _ in Parse.Char(';')
            select exp;

        public static readonly Parser<Statements> Statements =
            from statements in Statement.Many()
            select new Statements(statements);

        public static readonly Parser<Statements> Body =
            from ws1 in Parse.WhiteSpace.Many()
            from lBrace in Parse.Char('{')
            from ws2 in Parse.WhiteSpace.Many()
            from innerStatements in Parse.Ref(() => Statements)
            from ws3 in Parse.WhiteSpace.Many()
            from rBrace in Parse.Char('}')
            from ws4 in Parse.WhiteSpace.Many()
            select innerStatements;

        //private static readonly Parser<Args> Args = Parse..            

        //private static readonly Parser<IExpression> Invocation =
        //    from identifiers in Identifiers
        //    from args in ParenGroup
        //    from body in Body
        //    select new Invocaton(identifiers, args, body);
    }
}