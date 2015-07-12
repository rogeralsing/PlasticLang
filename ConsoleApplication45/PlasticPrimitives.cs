using System.Linq;
using PlasticLangLabb1.Ast;
using Sprache;

namespace PlasticLangLabb1
{
    public static class PlasticPrimitives
    {
        public static readonly Parser<Identifier> Identifier =
            from leading in Parse.WhiteSpace.Many()
            from first in Parse.Letter.Once()
            from rest in Parse.LetterOrDigit.Many()
            from trailing in Parse.WhiteSpace.Many()
            let token = new string(first.Concat(rest).ToArray())
            select new Identifier(token);

        public static readonly Parser<Identifiers> Identifiers =
            from ids in Parse.Ref(() => Identifier).AtLeastOnce()
            select new Identifiers(ids);

        public static readonly Parser<Number> Number =
            from leading in Parse.WhiteSpace.Many()
            from numb in Parse.DecimalInvariant
            from trailing in Parse.WhiteSpace.Many()
            select new Number(numb);

        public static readonly Parser<QuotedString> QuotedString =
            from leading in Parse.WhiteSpace.Many()
            from q1 in Parse.Char('"').Once()
            from str in Parse.Letter.Many()
            from q2 in Parse.Char('"').Once()
            from trailing in Parse.WhiteSpace.Many()
            select new QuotedString(new string(str.ToArray()));
    }
}
