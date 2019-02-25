using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ExpressionParser.Tests
{
    [TestFixture]
    class PerformanceTests
    {
        readonly RpnCalculator m_RpnCalculator = new RpnCalculator();

        [Test]
        public void Test_PerfTest_1()
        {
            var sw = new Stopwatch();

            const string rpnExpression = "12 + 3 * 4 + 2";
            var resultA = 0.0d;
            var resultB = 0.0d;
            var resultC = 0.0d;
            var n = 10000;

            Console.WriteLine("Native");
            {
                sw.Restart();
                for (int i = 0; i < n; i++)
                {
                    resultC += (12 + 3*4 + 2);
                }
                sw.Stop();
                Console.WriteLine("Time taken: {0}ms", sw.ElapsedMilliseconds);
            }

            Console.WriteLine("All Stages");
            {
                sw.Restart();
                for (int i = 0; i < n; i++)
                {
                    var evaluationContext = m_RpnCalculator.Compile(rpnExpression);
                    var output = m_RpnCalculator.Evaluate(evaluationContext);
                    resultA += output;
                }
                sw.Stop();
                Console.WriteLine("Time taken: {0}ms", sw.ElapsedMilliseconds);
            }

            Console.WriteLine("Precompile");
            {
                sw.Restart();
                var evaluationContext = m_RpnCalculator.Compile(rpnExpression);

                for (int i = 0; i < n; i++)
                {
                    var output = m_RpnCalculator.Evaluate(evaluationContext);
                    resultB += output;
                }
                sw.Stop();
                Console.WriteLine("Results: {0}", resultB);
                Console.WriteLine("Time taken: {0}ms", sw.ElapsedMilliseconds);
            }

            Assert.IsTrue(resultA == resultB);
            Assert.IsTrue(resultB == resultC);
        }

        [Test]
        public void Test_PerfTest_2()
        {
            Console.WriteLine("Perf test: Power of");

            var sw = new Stopwatch();

            const string infixExpression = "3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3";
            var resultA = 0.0d;
            var resultB = 0.0d;
            var outputA = 0.0d;
            var outputB = 0.0d;
            var n = 10000;

            Console.WriteLine("All Stages");
            {
                sw.Restart();
                for (int i = 0; i < n; i++)
                {
                    var evaluationContext = m_RpnCalculator.Compile(infixExpression);
                    outputA = m_RpnCalculator.Evaluate(evaluationContext);
                    resultA += outputA;
                }
                sw.Stop();
                Console.WriteLine("Time taken: {0}ms", sw.ElapsedMilliseconds);
            }

            Console.WriteLine("Precompile");
            {
                sw.Restart();
                var evaluationContext = m_RpnCalculator.Compile(infixExpression);

                for (int i = 0; i < n; i++)
                {
                    outputB = m_RpnCalculator.Evaluate(evaluationContext);
                    resultB += outputB;
                }
                sw.Stop();
                Console.WriteLine("Time taken: {0}ms", sw.ElapsedMilliseconds);
            }

            Assert.IsTrue(resultA == resultB);
            Assert.IsTrue(outputA == outputB);

            var approxEqual = TestHelper.ApproxEqual(outputA, 3, 0.001);
            Assert.IsTrue(approxEqual);
        }


        [Test]
        public void Test_PerfTest_3()
        {
            Console.WriteLine("Performance Test 3: Function Calls");

            var sw = new Stopwatch();

            const string infixExpression = "2*cos(0.0)*cos(0.0) + sin(0.0) - 1.0";
            Double outputA = 0.0f;
            Double outputB = 0.0f;
            Double sumA = 0.0f;
            Double sumB = 0.0f;

            {
                sw.Restart();
                Console.WriteLine("All Stages");
                for (int i = 0; i < 10000; i++)
                {
                    var evaluationContext = m_RpnCalculator.Compile(infixExpression);

                    outputA = m_RpnCalculator.Evaluate(evaluationContext);
                    sumA += outputA;
                }
                sw.Stop();
                Console.WriteLine("Time taken: {0}ms", sw.ElapsedMilliseconds);
            }

            {
                sw.Restart();
                Console.WriteLine("Precompile");
                var evaluationContext = m_RpnCalculator.Compile(infixExpression);
                for (int i = 0; i < 10000; i++)
                {
                    outputB = m_RpnCalculator.Evaluate(evaluationContext);
                    sumB += outputA;
                }
                sw.Stop();
                Console.WriteLine("Time taken: {0}ms", sw.ElapsedMilliseconds);
            }

            Assert.IsTrue(outputA == 1.0);
            Assert.IsTrue(outputA == outputB);
            Assert.IsTrue(sumA == sumB);
        }

        [Test]
        public void TestInfixToRpn_001()
        {
            Console.WriteLine("Performance Test 4: SymbolTable");

            var sw = new Stopwatch();

            const string infixExpression = "x^2 + 2";
            Double outputA = 0.0f;
            Double outputB = 0.0f;
            Double sumA = 0.0f;
            Double sumB = 0.0f;

            {
                sw.Restart();
                Console.WriteLine("All Stages");
                for (int i = 0; i < 10000; i++)
                {
                    m_RpnCalculator.RegisterSymbol("x");
                  
                    var evaluationContext = m_RpnCalculator.Compile(infixExpression);
                    evaluationContext.SymbolTable.RegisterSymbol("x",4);
                   
                    outputA = m_RpnCalculator.Evaluate(evaluationContext);
                    sumA += outputA;
                }
                sw.Stop();
                Console.WriteLine("Time taken: {0}ms", sw.ElapsedMilliseconds);
            }

            m_RpnCalculator.ClearAllSymbols();

            {
                sw.Restart();
                Console.WriteLine("Precompile");
                m_RpnCalculator.RegisterSymbol("x");
                var evaluationContext = m_RpnCalculator.Compile(infixExpression);
                for (int i = 0; i < 10000; i++)
                {
                    evaluationContext.SymbolTable.RegisterSymbol("x", 4);
                    outputB = m_RpnCalculator.Evaluate(evaluationContext);
                    sumB += outputA;
                }
                sw.Stop();
                Console.WriteLine("Time taken: {0}ms", sw.ElapsedMilliseconds);
            }

            Assert.IsTrue(outputA == 18.0);
            Assert.IsTrue(outputA == outputB);
            Assert.IsTrue(sumA == sumB);
        }
    }
}
