using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace ExpressionParser.Tests
{
    [TestFixture]
    internal class TestIncorrectInput
    {
        private readonly RpnCalculator m_RpnCalculator = new RpnCalculator();

        [Test]
        public void Test()
        {
            Assert.IsTrue(true);
        }



        [Test]
        public void TestInfixToRpn_001()
        {

            Assert.Throws<ExpressionParserException>(() =>
            {

                const string infixExpression = "cos(0.0";
                var evaluationContext = m_RpnCalculator.Compile(infixExpression);

                var ouput = m_RpnCalculator.Evaluate(evaluationContext);
                Assert.IsTrue(ouput == 1.0);

            });
        }

        [Test]
        public void TestInfixToRpn_002()
        {
            Assert.Throws<ExpressionParserException>(() =>
            {
                const string infixExpression = "";
                var evaluationContext = m_RpnCalculator.Compile(infixExpression);
                var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            });
        }
    }
}
