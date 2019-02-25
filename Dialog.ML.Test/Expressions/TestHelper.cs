using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser.Tests
{
    class TestHelper
    {
        public static Boolean ApproxEqual(double value, double expectedValue, double acceptedError)
        {
            var low = (expectedValue - acceptedError);
            var high = (expectedValue + acceptedError);
            var a = value > low;
            var b = value < high;
            return a && b;
        }
    }
}
