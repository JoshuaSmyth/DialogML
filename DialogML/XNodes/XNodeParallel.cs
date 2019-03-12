using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    public class XNodeParallel : XmlNode
    {
        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {

        }

        public override void WriteBytes(BinaryWriter bw, ref StringTable st)
        {
            base.WriteHeader(bw, XNodeType.Parallel);
        }
    }
}
