using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    public class XNodeExit : XmlNode
    {
        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            if(name == "id")
            {
                Id = ids.GetGuidByIndex(value);
            }
        }

        public override void WriteBytes(BinaryWriter bw, ref StringTable stringTable)
        {
            base.WriteHeader(bw, XNodeType.Exit);
        }
    }
}
