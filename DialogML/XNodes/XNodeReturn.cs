using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    public class XNodeReturn : XmlNode
    {
        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            if(name == "id")
            {
                Id = ids.GetGuidByIndex(value);
            }
        }

        public override void WriteBytes(CompileContext ctx)
        //public override void WriteBytes(BinaryWriter bw, string filename, ref StringTable stringTable, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(ctx.bw, XNodeType.Return);
        }
    }
}
