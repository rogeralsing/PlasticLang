
using PlasticLang;
using PlasticLang.Ast;
using Sprache;
using Xunit;

namespace PlasticSpec
{

    public class UnitTest1
    {
        [Fact]
        public void Can_parse_integer_number()
        {
            var number = PlasticParser.Expression.Parse("  123  ");
        }

        [Fact]
        public void Can_parse_decimal_number()
        {
            var number = PlasticParser.Expression.Parse("  123.456  ");
            Assert.True(number is NumberLiteral);
        }

        [Fact]
        public void Can_parse_identifier()
        {
            var identifier = PlasticParser.Expression.Parse("  abc  ");
            Assert.True(identifier is Symbol);
        }

        [Fact]
        public void Can_parse_identifiers()
        {
            var identifier = PlasticParser.Expression.Parse("  abc def ghi jkl ");
            Assert.True(identifier is ListValue);
        }

        [Fact]
        public void Can_parse_invocation()
        {
            var invocation = PlasticParser.Statement.Parse("  abc def ghi jkl ()  { print(x); } (x) ;  ");
            Assert.True(invocation is ListValue);
        }

        [Fact]
        public void Can_parse_terminating_invocation()
        {
            var invocation = PlasticParser.Statement.Parse("  abc def ghi jkl ()  { print(x); } ");
            Assert.True(invocation is ListValue);
        }

        [Fact]
        public void Can_parse_string()
        {
            var str = PlasticParser.Expression.Parse("  \"hej hopp 12334 !%¤%¤ \"  ");
            Assert.True(str is StringLiteral);
        }

        [Fact]
        public void Can_parse_addition()
        {
            var addition = PlasticParser.Expression.Parse("  a+b  ");
            Assert.True(addition is ListValue);
        }

        [Fact]
        public void Can_parse_subtraction()
        {
            var subtraction = PlasticParser.Expression.Parse("  a-b  ");
            Assert.True(subtraction is ListValue);
        }

        [Fact]
        public void Can_parse_multiplication()
        {
            var multiplication = PlasticParser.Expression.Parse("  a*b  ");
            Assert.True(multiplication is ListValue);
        }

        [Fact]
        public void Can_parse_division()
        {
            var division = PlasticParser.Expression.Parse("  a/b  ");
            Assert.True(division is ListValue);
        }

        [Fact]
        public void Can_parse_assignment()
        {
            var assignment = PlasticParser.Expression.Parse("   a:=2  ");
            Assert.True(assignment is ListValue);
        }

        [Fact]
        public void Can_parse_assignment_assignment()
        {
            var assignment = PlasticParser.Expression.Parse("  a:=b:=c  ");
            Assert.True(assignment is ListValue);
        }
        

        [Fact]
        public void Can_parse_assignment_lambda()
        {
            var assignment = PlasticParser.Expression.Parse("   a := x => b  ");
            Assert.True(assignment is ListValue);
        }

        [Fact]
        public void Can_parse_lambda_declaration_()
        {
            PlasticParser.Expression.Parse("  () => {y;} ");
            PlasticParser.Expression.Parse("  x => y  ");
            PlasticParser.Expression.Parse("  (x) => {y;} ");
            PlasticParser.Expression.Parse("  (a,b) => y  ");
        }

        [Fact]
        public void Can_parse_body()
        {
            var lambda1 = PlasticParser.Body.Parse(" { x; }");
            //    var lambda2 = PlasticParser.Expression.Parse("  x => y  ");
        }

        [Fact]
        public void Can_parse_empty_body()
        {
            var statement = PlasticParser.Body.Parse(" {  }  ");
            //    var lambda2 = PlasticParser.Expression.Parse("  x => y  ");
        }
    }
}