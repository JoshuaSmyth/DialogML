using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ExpressionParser.Tests
{
    public delegate void ExampleCallback(double lineCount);

    public class WorkItem
    {
        private readonly ManualResetEvent m_Sync;
        private readonly int m_From;
        private readonly int m_To;
        private readonly RpnCalculator m_Calculator;
        private readonly double[] m_Results;
        private readonly int m_Index;
        private EvaluationContext m_Context;
        private readonly ExampleCallback m_Callback;

        public WorkItem(ManualResetEvent sync, Int32 from, Int32 to, RpnCalculator calculator, ExampleCallback callback)
        {
            m_Sync = sync;
            m_From = from;
            m_To = to;
            m_Calculator = calculator;
            m_Callback = callback;
        }
        public WorkItem(ManualResetEvent sync, Int32 from, Int32 to, RpnCalculator calculator, double[] results, Int32 index)
        {
            m_Sync = sync;
            m_From = from;
            m_To = to;
            m_Calculator = calculator;
            m_Results = results;
            m_Index = index;
            
        }
        public EvaluationContext Context
        {
            get { return m_Context; }
            set { m_Context = value; }
        }


        public void ThreadProc()
        {
            var rv = 0.0d;
            for (int i = m_From; i < m_To; i++)
            {
                rv +=  m_Calculator.Evaluate(Context);
            }
            if (m_Results != null)
                m_Results[m_Index] = rv;

            if (m_Callback != null)
                m_Callback(rv);
            m_Sync.Set();
        }
    }

    [TestFixture]
    internal class TestThreadSafety
    {
        private readonly RpnCalculator m_RpnCalculator = new RpnCalculator();

        [Test]
        public void Test()
        {
            Assert.IsTrue(true);
        }



        [Test]
        public void TestThreadSafety_001()
        {
            var sw = new Stopwatch();
            var n = 100000;

            const string rpnExpression = "12 + 3 * 4 + 2";
            var evaluationContext = m_RpnCalculator.Compile(rpnExpression);
            sw.Restart();
            var baseResult = 0.0d;
          
            for (int i = 0; i < n; i++)
            {
                baseResult += m_RpnCalculator.Evaluate(evaluationContext);
            }
            Console.WriteLine("base time:{0} ms",sw.ElapsedMilliseconds);
            sw.Stop();



            var result = 0.0d;
            var resultA = 0.0d;
            var resultB = 0.0d;
            var resultC = 0.0d;
            var resultD = 0.0d;

            var resetEventA = new ManualResetEvent(false);
            var resetEventB = new ManualResetEvent(false);
   
            var workItem = new WorkItem(resetEventA, 0, n/2, m_RpnCalculator, o => resultA = o);
            var workItemB = new WorkItem(resetEventB, n/2, n, m_RpnCalculator, o => resultB = o);
           
            var thread1 = new Thread(new ThreadStart(workItem.ThreadProc));
            var thread2 = new Thread(new ThreadStart(workItemB.ThreadProc));
            thread1.Priority = ThreadPriority.Highest;
            thread2.Priority = ThreadPriority.Highest;

           
            Console.WriteLine("Precompile");
            {
                var evaluationContextA = m_RpnCalculator.Compile(rpnExpression);
                var evaluationContextB = m_RpnCalculator.Compile(rpnExpression);

                workItem.Context = evaluationContextA;
                workItemB.Context = evaluationContextB;
              
                sw.Restart();
              
                thread1.Start();
                thread2.Start();
           
                

                resetEventA.WaitOne();
                resetEventB.WaitOne();


                sw.Stop();

                result = resultA + resultB + resultC + resultD;
                Console.WriteLine("result :{0}", result);
            
                Console.WriteLine("Time taken: {0}ms", sw.ElapsedMilliseconds);
            }
            Assert.IsTrue(true);
            Assert.IsTrue(result == baseResult);
        }

        [Test]
        public void TestThreadSafety_002()
        {
            var sw = new Stopwatch();
            var n = 100000;

            const string rpnExpression = "12 + 3 * 4 + 2";
            var evaluationContext = m_RpnCalculator.Compile(rpnExpression);
            sw.Restart();
            var baseResult = 0.0d;

            for (int i = 0; i < n; i++)
            {
                baseResult += m_RpnCalculator.Evaluate(evaluationContext);
            }
            Console.WriteLine("base time:{0} ms", sw.ElapsedMilliseconds);
            sw.Stop();

            var finalResult = 0.0d;
            var resultA = 0.0d;
            var resultB = 0.0d;

            var resetEventA = new ManualResetEvent(false);
            var resetEventB = new ManualResetEvent(false);

            var workItem = new WorkItem(resetEventA, 0, n / 2, m_RpnCalculator, o => resultA = o);
            var workItemB = new WorkItem(resetEventB, n / 2, n, m_RpnCalculator, o => resultB = o);

            var thread1 = new Thread(workItem.ThreadProc);
            var thread2 = new Thread(workItemB.ThreadProc);
            thread1.Priority = ThreadPriority.Highest;
            thread2.Priority = ThreadPriority.Highest;


            Console.WriteLine("Precompile");
            {
                var evaluationContextA = m_RpnCalculator.Compile(rpnExpression);
                var evaluationContextB = m_RpnCalculator.Compile(rpnExpression);

                workItem.Context = evaluationContextA;
                workItemB.Context = evaluationContextB;

                sw.Restart();

                thread1.Start();
                thread2.Start();

                resetEventA.WaitOne();
                resetEventB.WaitOne();

                sw.Stop();

                finalResult = resultA + resultB;
                Console.WriteLine("result :{0}", finalResult);

                Console.WriteLine("threaded Time taken: {0}ms", sw.ElapsedMilliseconds);
            }
            Assert.IsTrue(true);
            Assert.IsTrue(finalResult == baseResult);
        }

        [Test]
        public void TestThreadSafety_003()
        {
            var sw = new Stopwatch();
            var n = 100000;

            const string rpnExpression = "12 + 3 * 4 + 2";
            var evaluationContext = m_RpnCalculator.Compile(rpnExpression);
            sw.Restart();
            var baseResult = 0.0d;

            for (int i = 0; i < n; i++)
            {
                baseResult += m_RpnCalculator.Evaluate(evaluationContext);
            }
            Console.WriteLine("base time:{0} ms", sw.ElapsedMilliseconds);
            sw.Stop();

            var results = new double[2] {0, 0};
            var finalResult = 0.0d;
            var resultA = 0.0d;
            var resultB = 0.0d;

            var resetEventA = new ManualResetEvent(false);
            var resetEventB = new ManualResetEvent(false);

            var workItem = new WorkItem(resetEventA, 0, n / 2, m_RpnCalculator, results, 0);
            var workItemB = new WorkItem(resetEventB, n / 2, n, m_RpnCalculator, results, 1);

            var thread1 = new Thread(workItem.ThreadProc);
            var thread2 = new Thread(workItemB.ThreadProc);
            thread1.Priority = ThreadPriority.Highest;
            thread2.Priority = ThreadPriority.Highest;


            Console.WriteLine("Precompile");
            {
                var evaluationContextA = m_RpnCalculator.Compile(rpnExpression);
                var evaluationContextB = m_RpnCalculator.Compile(rpnExpression);

                workItem.Context = evaluationContextA;
                workItemB.Context = evaluationContextB;

                sw.Restart();

                thread1.Start();
                thread2.Start();

                resetEventA.WaitOne();
                resetEventB.WaitOne();

                sw.Stop();

                finalResult = results[0] + results[1];
                Console.WriteLine("result :{0}", finalResult);

                Console.WriteLine("threaded Time taken: {0}ms", sw.ElapsedMilliseconds);
            }
            Assert.IsTrue(true);
            Assert.IsTrue(finalResult == baseResult);
        }
    }
}
