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
        public CompiledExpression Expression;
        
        public void OnProcessElement(ScriptIds ids, string name, string value)
        {
            var loweredName = name.ToLower();
            if(loweredName == "expression")
            {
                
                var expressionParser = new RpnCompiler(new HostCallTable());
                var tokens = expressionParser.ConvertToReversePolishNotation(value);
                var tokenStream = expressionParser.ConvertToBytestream(tokens);
                Expression = new CompiledExpression(tokenStream);
                
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

        public override void WriteBytes(BinaryWriter bw, ref StringTable st)
        {
            base.WriteHeader(bw, XNodeType.If);


            // TODO 
            bw.Write(Expression.Bytes.Length);
            bw.Write(Expression.Bytes);

            // TODO This should be added to the expression table
            // Not the string table
            //st.AddString(this.Id, this.Expression);
        }
    }
}
