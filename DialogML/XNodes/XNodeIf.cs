using DialogML.Expressions;
using ExpressionParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    class XNodeIf : XmlNode
    {
        public String Expression;

        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            var loweredName = name.ToLower();
            if(loweredName == "expression")
            {
                Expression = value;

            }
            else if(loweredName == "id")
            {
                if(value == "{guid}")
                {
                    Id = Guid.NewGuid();
                }
                else
                {
                    Id = ids.GetGuidByIndex(value); //Guid.Parse(value);
                }
            }
        }

        public override void WriteBytes(BinaryWriter bw, string filename, ref StringTable st, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(bw, XNodeType.If);

            if(Expression == null)
            {
                bw.Write(0);
            }
            else
            {
                // TODO Pass in an RPN Compiler
                var expressionParser = new RpnCompiler(new HostCallTable());
                var tokens = expressionParser.ConvertToReversePolishNotation(Expression);
                var tokenStream = expressionParser.ConvertToBytestream(tokens);
                var CompiledExpression = new CompiledExpression(tokenStream);
                
                bw.Write(CompiledExpression.Bytes.Length);
                bw.Write(CompiledExpression.Bytes);
            }
        }
    }
}
