using System;
using System.IO;

namespace DialogML.XNodes
{
    public class XNodeCaseTrue : XmlNode
    {
        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            var loweredName = name.ToLower();
            if(loweredName == "id")
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

        public override void WriteBytes(CompileContext ctx)
        //public override void WriteBytes(BinaryWriter bw, string filename, ref StringTable st, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(ctx.bw, XNodeType.CaseTrue);
        }
    }
}
