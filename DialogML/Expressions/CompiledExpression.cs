using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.Expressions
{
    public class CompiledExpression
    {
        public byte[] Bytes;

        public CompiledExpression(byte[] bytes)
        {
            Bytes = bytes;
        }
    }
}
