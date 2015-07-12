using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlasticLangLabb1;
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
        }

        [TestMethod]
        public void Can_parse_identifier()
        {
            var identifier = PlasticParser.Expression.Parse("  abc  ");
        }

        [TestMethod]
        public void Can_parse_string()
        {
            var str = PlasticParser.Expression.Parse("  \"hej hopp 12334 !%¤%¤ \"  ");
        }

        [TestMethod]
        public void Can_parse_addition()
        {
            var addition = PlasticParser.Expression.Parse("  a+b  ");
        }

        [TestMethod]
        public void Can_parse_subtraction()
        {
            var addition = PlasticParser.Expression.Parse("  a-b  ");
        }

        [TestMethod]
        public void Can_parse_multiplication()
        {
            var multiplication = PlasticParser.Expression.Parse("  a*b  ");
        }

        [TestMethod]
        public void Can_parse_division()
        {
            var division = PlasticParser.Expression.Parse("  a/b  ");
        }

        [TestMethod]
        public void Can_parse_assignment()
        {
            var division = PlasticParser.Expression.Parse("  let a=2  ");
        }

        [TestMethod]
        public void Can_parse_lambda_declaration_()
        {
            var lambda1 = PlasticParser.Expression.Parse("  (x) => {y;} ");
            var lambda2 = PlasticParser.Expression.Parse("  x => y  ");
        }
    }
}
