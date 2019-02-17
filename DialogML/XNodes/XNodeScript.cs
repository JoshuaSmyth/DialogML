using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    class XScript : XmlNode
    {
        public override void WriteBytes(BinaryWriter bw, ref StringTable st)
        {
            bw.Write((ushort)XNodeType.Script);
            bw.Write((byte)1);    // Version Major
            bw.Write((byte)0);    // Version Minor
            bw.Write((ushort)(Children?.Count ?? 0));

            bw.Write(this.Id.ToByteArray());
            bw.Write(this.Name);
        }
    }
}
