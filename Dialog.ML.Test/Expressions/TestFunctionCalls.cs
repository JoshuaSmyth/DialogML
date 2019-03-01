using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ExpressionParser.Tests
{
    [TestFixture]
    internal class TestFunctionCalls
    {
        private readonly RpnCalculator m_RpnCalculator = new RpnCalculator();

        [OneTimeSetUp]
        public void SetupFixture()
        {
            // TODO symbol ids should be guids
            m_RpnCalculator.RegisterSymbol("actor.joel.isdead");

            // TODO This function should take a guid to represent the symbol id
            m_RpnCalculator.RegisterFunction("isSet", new Func<Double, Double>((id) => { return 0; }));
        }

        [Test]
        public void Test()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void TestInfixToRpn_001()
        {
            const string infixExpression = "cos(0.0) + sin(0.0)";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.0);
        }

        [Test]
        public void TestInfixToRpn_002()
        {
            const string infixExpression = "2*cos(0.0)*cos(0.0) + sin(0.0) - 1.0";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.0);
        }

        [Test]
        public void TestInfixToRpn_003()
        {
            const string infixExpression = "sin(cos(0.0)) - sin(cos(0.0))";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 0.0);
        }

        [Test]
        public void TestInfixToRpn_004()
        {
            const string infixExpression = "cos(0.0) - exp(0.0)";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 0.0);
        }

        [Test]
        public void TestInfixToRpn_005()
        {
            const string infixExpression = "max(0.5, 1.0)";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.0);
        }

        [Test]
        public void TestInfixToRpn_006()
        {
            const string infixExpression = "max(0.5, cos(0.0))";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.0);
        }

        [Test]
        public void TestInfixToRpn_007()
        {
            const string infixExpression = "2*cos(0.0)*cos(0.0) + sin(0.0) - 1.0/(0+4)";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.75);
        }

        [Test]
        public void TestInfixToRpn_008()
        {
            const string infixExpression = "isset(actor.joel.isdead)";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 0);
        }

        [Test]
        public void TestInfixToRpn_009()
        {
            // TODO Define Flag
            // TODO Define Var
            // TODO Assign Var
            // TODO Assign Flag


            const string infixExpression = "!isset(actor.joel.isdead)";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1);
        }
    }
}
