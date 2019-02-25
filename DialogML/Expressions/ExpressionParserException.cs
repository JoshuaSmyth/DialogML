using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser
{
    public class ExpressionParserException : Exception
    {
        public ExpressionParserException(string message) : base(message)
        {

        }
    }
}
