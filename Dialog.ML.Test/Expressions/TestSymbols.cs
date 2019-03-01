using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ExpressionParser.Tests
{
    [TestFixture]
    internal class TestSymbols
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
            // TODO These symbols have to be registered as guids
            // TODO The preparser needs to pick them up

            m_RpnCalculator.RegisterSymbol("x");

            const string infixExpression = "x^2 + 2";
      
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);
            evaluationContext.SetSymbol("x", 4);
        
            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 18.0);
        }

        [Test]
        public void TestInfixToRpn_002()
        {
            m_RpnCalculator.RegisterSymbol("x");

            const string infixExpression = "x^2 + 2";
   
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);
            evaluationContext.SetSymbol("x", 4);
            evaluationContext.SetSymbol("x", 4);
            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 18.0);
        }

        [Test]
        public void TestInfixToRpn_003()
        {
            m_RpnCalculator.RegisterSymbol("age");

            const string infixExpression = "age > 10 && age < 20";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            evaluationContext.SetSymbol("age", 15);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.0);
        }
    }
}
