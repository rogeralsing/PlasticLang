﻿using System.Linq;
using Sprache;

namespace PlasticLangLabb1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var identifier =
                from leading in Parse.WhiteSpace.Many()
                from first in Parse.Letter.Once()
                from rest in Parse.LetterOrDigit.Many()
                from trailing in Parse.WhiteSpace.Many()
                let token = new string(first.Concat(rest).ToArray())
                select new Identifier(token) as IExpression;

            var identifiers =
                from ids in identifier.AtLeastOnce()
                select new Identifiers(ids);

            var number =
                from leading in Parse.WhiteSpace.Many()
                from numb in Parse.DecimalInvariant
                from trailing in Parse.WhiteSpace.Many()
                select new Number(numb) as IExpression;

            var quotedString =
                from leading in Parse.WhiteSpace.Many()
                from q1 in Parse.Char('"').Once()
                from str in Parse.Letter.Many()
                from q2 in Parse.Char('"').Once()
                from trailing in Parse.WhiteSpace.Many()
                select new QuotedString(new string(str.ToArray())) as IExpression;

            var literal =
                identifiers
                    .Or(number)
                    .Or(quotedString);

            var multiplyOperator = Parse.String("*").Token().Return(new MultiplyBinary() as BinaryOperator);
            var divideOperator = Parse.String("/").Token().Return(new DivideBinary() as BinaryOperator);
            var addOperator = Parse.String("+").Token().Return(new AddBinary() as BinaryOperator);
            var subtractOperator = Parse.String("-").Token().Return(new SubtractBnary() as BinaryOperator);

            var innerTerm = Parse.ChainOperator(addOperator.Or(subtractOperator), literal,
                (o, l, r) => new BinaryExpression(l, o, r));

            var term = Parse.ChainOperator(multiplyOperator.Or(divideOperator), innerTerm,
                (o, l, r) => new BinaryExpression(l, o, r));

            var expression = term;

            var parenGroup =
                from lPar in Parse.Char('(').Once()
                from exp in expression.Once()
                from rPar in Parse.Char(')').Once()
                select exp;

            var statement =
                from exp in expression
                from _ in Parse.Char(';')
                select exp;

            var res = statement.Parse("  555 + \"abcd\" + \r\n leif rolf olle;");
        }
    }
}