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
    public class XNodeOption : XmlNode
    {
        public string Text;
        public bool RemoveOnSelect = false;
        public string Expression;

        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            var loweredName = name.ToLower();
            if(loweredName == "text")
            {
                Text = value;
            }
            else if (loweredName == "id")
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
            else if (loweredName == "remove-on-select")
            {
                RemoveOnSelect = bool.Parse(value);
            }
            else if(loweredName == "only-if")
            {
                Expression = value;
            }
        }

        public override void WriteBytes(BinaryWriter bw, string fileName, ref StringTable st, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(bw, XNodeType.Option);

            st.AddString(this.Id, this.Text);
            bw.Write(RemoveOnSelect);
            if (Expression == null)
            {
                bw.Write((byte)0);
            }
            else
            {
                bw.Write((byte)1);

                // TODO Pass in an RPN Compiler
                var expressionParser = new RpnCompiler(new HostCallTable());
                var tokens = expressionParser.ConvertToReversePolishNotation(Expression);
                var tokenStream = expressionParser.ConvertToBytestream(tokens);
                var compiledExpression = new CompiledExpression(tokenStream);

                bw.Write(compiledExpression.Bytes.Length);
                bw.Write(compiledExpression.Bytes);
            }
        }
    }
}
