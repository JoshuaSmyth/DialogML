using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    class XNodeVars : XmlNode
    {
        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            // NOOP
        }

        public override void WriteBytes(BinaryWriter bw, string filename, ref StringTable stringTable, ref ReferencesTable referencesTable)
        {
            //bw.Write((byte)XNodeType.Noop);
            // NOOP
            base.WriteHeader(bw, XNodeType.Noop);

            //bw.Write(this.Name ?? "null");
        }
    }
}
