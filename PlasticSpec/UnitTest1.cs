using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlasticLangLabb1;
using PlasticLangLabb1.Ast;
using Sprache;

namespace PlasticSpec
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_parse_integer_number()
        {
            var number = PlasticParser.Expression.Parse("  123  ");
        }

        [TestMethod]
        public void Can_parse_decimal_number()
        {
            var number = PlasticParser.Expression.Parse("  123.456  ");
            Assert.IsTrue(number is Number);
        }

        [TestMethod]
        public void Can_parse_identifier()
        {
            var identifier = PlasticParser.Expression.Parse("  abc  ");
            Assert.IsTrue(identifier is Identifiers);
        }

        [TestMethod]
        public void Can_parse_identifiers()
        {
            var identifier = PlasticParser.Expression.Parse("  abc def ghi jkl ");
            Assert.IsTrue(identifier is Identifiers);
        }

        [TestMethod]
        public void Can_parse_invocation()
        {
            var invocation = PlasticParser.Statement.Parse("  abc def ghi jkl ()  { print(x); } (x) ;  ");
            Assert.IsTrue(invocation is Invocaton);
        }

        [TestMethod]
        public void Can_parse_terminating_invocation()
        {
            var invocation = PlasticParser.Statement.Parse("  abc def ghi jkl ()  { print(x); } ");
            Assert.IsTrue(invocation is Invocaton);
        }

        [TestMethod]
        public void Can_parse_string()
        {
            var str = PlasticParser.Expression.Parse("  \"hej hopp 12334 !%¤%¤ \"  ");
            Assert.IsTrue(str is QuotedString);
        }

        [TestMethod]
        public void Can_parse_addition()
        {
            var addition = PlasticParser.Expression.Parse("  a+b  ");
            Assert.IsTrue(addition is BinaryExpression);
        }

        [TestMethod]
        public void Can_parse_subtraction()
        {
            var subtraction = PlasticParser.Expression.Parse("  a-b  ");
            Assert.IsTrue(subtraction is BinaryExpression);
        }

        [TestMethod]
        public void Can_parse_multiplication()
        {
            var multiplication = PlasticParser.Expression.Parse("  a*b  ");
            Assert.IsTrue(multiplication is BinaryExpression);
        }

        [TestMethod]
        public void Can_parse_division()
        {
            var division = PlasticParser.Expression.Parse("  a/b  ");
            Assert.IsTrue(division is BinaryExpression);
        }

        [TestMethod]
        public void Can_parse_assignment()
        {
            var assignment = PlasticParser.Expression.Parse("  let a=2  ");
            Assert.IsTrue(assignment is LetAssignment);
        }

        [TestMethod]
        public void Can_parse_assignment_assignment()
        {
            var assignment = PlasticParser.Expression.Parse("  let a=b=c  ");
            Assert.IsTrue(assignment is LetAssignment);
        }

        [TestMethod]
        public void Can_parse_multi_assignment()
        {
            var assignment = PlasticParser.Expression.Parse("  let a, b, c=2 ");
            Assert.IsTrue(assignment is LetAssignment);
        }

        [TestMethod]
        public void Can_parse_assignment_lambda()
        {
            var assignment = PlasticParser.Expression.Parse("  let a = () => b  ");
            Assert.IsTrue(assignment is LetAssignment);
        }

        [TestMethod]
        public void Can_parse_lambda_declaration_()
        {
            var lambda1 = PlasticParser.Expression.Parse("  (x) => {y;} ");
            var lambda2 = PlasticParser.Expression.Parse("  x => y  ");
        }

        [TestMethod]
        public void Can_parse_body()
        {
            var lambda1 = PlasticParser.Body.Parse(" {***}");
            //    var lambda2 = PlasticParser.Expression.Parse("  x => y  ");
        }

        [TestMethod]
        public void Can_parse_empty_statements()
        {
            var statement = PlasticParser.Statements.Parse("  ");
            //    var lambda2 = PlasticParser.Expression.Parse("  x => y  ");
        }

        [TestMethod]
        public void Can_parse_statements()
        {
            var statement = PlasticParser.Statements.Parse(" *** ");
            //    var lambda2 = PlasticParser.Expression.Parse("  x => y  ");
        }
    }
}