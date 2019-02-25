using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace ExpressionParser.Tests
{
    [TestFixture]
    class TestExpressionInput
    {
        readonly RpnCalculator m_RpnCalculator = new RpnCalculator();

        [Test]
        public void Test()
        {
            Assert.IsTrue(true);
        }


        [Test]
        public void TestInfixToRpn_001()
        {
            const string infixExpression = "1 / 2";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 0.5);
        }

        [Test]
        public void TestInfixToRpn_002()
        {
            const string infixExpression = "4 * 5 + 1 / 2";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 20.5);
        }

        [Test]
        public void TestInfixToRpn_003()
        {
            const string infixExpression = "3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(TestHelper.ApproxEqual(ouput, 3, 0.001));
        }

        [Test]
        public void TestInfixToRpn_004()
        {
            const string infixExpression = "-1 * ( 1 - 5 ) * 2.5";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 10);
        }
        
        [Test]
        public void TestInfixToRpn_005()
        {
            const string infixExpression = "( 1 - 5 )";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == -4);
        }

        [Test]
        public void TestInfixToRpn_006()
        {
            const string infixExpression = "( 1 - 5 ) * (6+2) / ( 1 - 2)*2";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 64);
        }

        [Test]
        public void TestInfixToRpn_007()
        {
            const string infixExpression = "( 1 - 5 )+1";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == -3);
        }

        [Test]
        public void TestInfixToRpn_008()
        {
            const string infixExpression = "( 1 - 5 )-1";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == -5);
        }

        [Test]
        public void TestInfixToRpn_009()
        {
            const string infixExpression = "-( 1 - 5 )-1";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 3);
        }

        [Test]
        public void TestInfixToRpn_010()
        {
            const string infixExpression = "-( 1 + -5)";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 4);
        }

        [Test]
        public void TestInfixToRpn_011()
        {
            const string infixExpression = "(1 + 5) % 4";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 2);
        }

        [Test]
        public void TestInfixToRpn_012()
        {
            const string infixExpression = "!0";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1);
        }

        [Test]
        public void TestInfixToRpn_013()
        {
            const string infixExpression = "!1";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 0);
        }

        [Test]
        public void TestInfixToRpn_014()
        {
            const string infixExpression = "!!(4+-4)*2+1";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.0);
        }

        [Test]
        public void TestInfixToRpn_015()
        {
            const string infixExpression = "2 > 1";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.0);
        }

        [Test]
        public void TestInfixToRpn_016()
        {
            const string infixExpression = "2 < 1";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 0.0);
        }

        [Test]
        public void TestInfixToRpn_017()
        {
            const string infixExpression = "2 >=2";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.0);
        }

        [Test]
        public void TestInfixToRpn_018()
        {
            const string infixExpression = "4 <=4";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.0);
        }
        
        [Test]
        public void TestInfixToRpn_019()
        {
            const string infixExpression = "4 ==4";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.0);
        }

        [Test]
        public void TestInfixToRpn_020()
        {
            const string infixExpression = "4 !=4";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 0.0);
        }

        [Test]
        public void TestInfixToRpn_021()
        {
            const string infixExpression = "4 || 0";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.0);
        }

        [Test]
        public void TestInfixToRpn_022()
        {
            const string infixExpression = "0 || 4";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.0);
        }

        [Test]
        public void TestInfixToRpn_023()
        {
            const string infixExpression = "0 &&(4-4)";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 0.0);
        }
        
        [Test]
        public void TestInfixToRpn_024()
        {
            const string infixExpression = "(1+2) &&(4-3)";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.0);
        }

        [Test]
        public void TestInfixToRpn_025()
        {
            const string infixExpression = "!(-2+2)&& (4*3)";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 1.0);
        }

        [Test]
        public void TestInfixToRpn_026()
        {
            const string infixExpression = "0 || 1 && 1 && 0";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 0.0);
        }

        [Test]
        public void TestInfixToRpn_027()
        {
            const string infixExpression = "10.5 + 10.5";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 21);
        }

        [Test]
        public void TestInfixToRpn_028()
        {
            const string infixExpression = "10.05 + 10.05";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 20.1);
        }

        [Test]
        public void TestInfixToRpn_029()
        {
            const string infixExpression = "0.05 + -10.05";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == -10);
        }

        [Test]
        public void TestInfixToRpn_030()
        {
            const string infixExpression = "(5 / 2) - -(3*6)";
            var evaluationContext = m_RpnCalculator.Compile(infixExpression);

            var ouput = m_RpnCalculator.Evaluate(evaluationContext);
            Assert.IsTrue(ouput == 20.5);
        }
    }
}
