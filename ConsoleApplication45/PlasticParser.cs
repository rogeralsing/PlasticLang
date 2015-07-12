using System.Collections.Generic;
using System.Linq;
using PlasticLangLabb1.Ast;
using Sprache;

namespace PlasticLangLabb1
{
    public class PlasticParser
    {
        private static Parser<string> TokenWithWS(string token)
        {
            var parser = from ws1 in Parse.WhiteSpace.Many()
                from t in Parse.String(token)
                from ws2 in Parse.WhiteSpace.Many()
                select new string(t.ToArray());

            return parser;
        }

        public static Parser<BinaryOperator> BinOp(string op, BinaryOperator node)
        {
            return Parse.String(op).Return(node);
        }

        public static readonly Parser<string> LambdaArrow = TokenWithWS("=>");
        public static readonly Parser<string> LParen = TokenWithWS("(");
        public static readonly Parser<string> RParen = TokenWithWS(")");
        public static readonly Parser<string> LBrace = TokenWithWS("{");
        public static readonly Parser<string> RBrace = TokenWithWS("}");
        public static readonly Parser<string> Comma = TokenWithWS(",");
        public static readonly Parser<string> SemiColon = TokenWithWS(";");
        public static readonly Parser<IEnumerable<char>> WS = Parse.WhiteSpace.Many();

        public static readonly Parser<char> LQoute =
            from ws in WS
            from q in Parse.Char('"')
            select q;

        public static readonly Parser<char> RQoute =
            from q in Parse.Char('"')
            from ws in WS
            select q;

        public static readonly Parser<Identifier> Identifier =
            from leading in WS
            from first in Parse.Letter.Once()
            from rest in Parse.LetterOrDigit.Many()
            from trailing in WS
            let token = new string(first.Concat(rest).ToArray())
            select new Identifier(token);

        public static readonly Parser<Identifiers> Identifiers =
            from ids in Parse.Ref(() => Identifier).AtLeastOnce()
            select new Identifiers(ids);

        public static readonly Parser<Number> Number =
            from leading in WS
            from numb in Parse.DecimalInvariant
            from trailing in WS
            select new Number(numb);

        public static readonly Parser<QuotedString> QuotedString =
            from str in Parse.CharExcept('"').Many().Contained(LQoute, RQoute)
            select new QuotedString(new string(str.ToArray()));

        public static readonly Parser<IExpression> Literal =
           Identifiers.Select(x => x as IExpression)
                .Or(Number)
                .Or(QuotedString);

        public static readonly Parser<IExpression> Value =
            Parse.Ref(() => ParenExpression)
                //      .Or(Parse.Ref(() => Invocation))
                .Or(Parse.Ref(() => Literal));

        public static readonly Parser<BinaryOperator> MultiplyOperator = BinOp("*", new MultiplyBinary());
        public static readonly Parser<BinaryOperator> DivideOperator = BinOp("/", new DivideBinary());
        public static readonly Parser<BinaryOperator> AddOperator = BinOp("+", new AddBinary());
        public static readonly Parser<BinaryOperator> SubtractOperator = BinOp("-", new SubtractBnary());
        public static readonly Parser<BinaryOperator> EqualsOperator = BinOp("==", new EqualsBinary());

        public static readonly Parser<IExpression> InnerTerm = Parse.ChainOperator(AddOperator.Or(SubtractOperator),
            Value, (o, l, r) => new BinaryExpression(l, o, r));

        public static readonly Parser<IExpression> Term = Parse.ChainOperator(MultiplyOperator.Or(DivideOperator),
            InnerTerm, (o, l, r) => new BinaryExpression(l, o, r));

        public static readonly Parser<IExpression> Compare = Parse.ChainOperator(EqualsOperator,
            Term, (o, l, r) => new BinaryExpression(l, o, r));

        public static readonly Parser<IExpression> Assign =
            from x in TokenWithWS("let")
            from cells in Identifier.DelimitedBy(Comma)
            from assignOp in TokenWithWS("=")
            from expression in Parse.Ref(() => Expression)
            select new LetAssignment(cells, expression);

        public static readonly Parser<IExpression> Expression =
            Parse.Ref(() => LambdaDeclaration)
                .Or(Parse.Ref(() => Assign))
                .Or(Parse.Ref(() => Compare));

        public static readonly Parser<IExpression> ParenExpression =
            Parse.Ref(() => Parse.Ref(() => Expression)).Contained(LParen, RParen);

        public static readonly Parser<IExpression> TerminatedStatement =
            from exp in Parse.Ref(() => Expression)
            from _ in Parse.Char(';')
            select exp;

        public static readonly Parser<IExpression> Statement =
            Parse.Ref(() => Body)
                .Or(Parse.Ref(() => TerminatedStatement));

        public static readonly Parser<Statements> Statements =
            from statements in Statement.Many()
            select new Statements(statements);

        public static readonly Parser<Statements> Body = Parse.Ref(() => Statements).Contained(LBrace, RBrace);
        public static readonly Parser<IExpression> LambdaBody = Parse.Ref(() => Body).Or(Parse.Ref(() => Expression));

        public static readonly Parser<IEnumerable<Identifier>> LambdaArgs =
            Identifier
                .DelimitedBy(Comma)
                .Optional()
                .Contained(LParen, RParen)
                .Select(o => o.GetOrDefault())
                .Or(Identifier.Once());

        public static readonly Parser<IExpression> LambdaDeclaration =
            from args in LambdaArgs
            from arrow in LambdaArrow
            from body in Parse.Ref(() => LambdaBody)
            select new LambdaDeclaration(args, body);

        public static readonly Parser<IEnumerable<IExpression>> InvocationArgs =
            Parse.Ref(() => Expression)
                .DelimitedBy(Comma)
                .Optional()
                .Contained(LParen, RParen)
                .Select(o => o.IsDefined ? o.Get() : Enumerable.Empty<Identifier>())
                .Or(Identifier.Once());
        
        //this is the iffy part
        //`abc def ghi` should be an `Identifiers`
        //`abc def ghi ()` should be an `Invocation`
        //`abc def ghi {}` should be an `Invocation`
        //`abc def ghi (){}` should be an `Invocation`
        //`abc def ghi (){} jkl ()` should be an `Invocation`
        //`abc def ghi (){} jkl ()` should be an `Invocation`
        private static readonly Parser<Invocaton> Invocation =
            from identifiers in Identifiers
            from args in InvocationArgs.Optional()
            from body in Body.Optional()
            select new Invocaton(identifiers, args.GetOrDefault(), body.GetOrDefault());

        private static readonly Parser<IExpression> Invocations =
            from invocations in Invocation.AtLeastOnce()
            select new Invocations(invocations);

    }
}