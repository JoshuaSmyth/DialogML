using System;
using System.IO;

namespace DialogML.XNodes
{
    class XNodePage : XmlNode
    {
        public override void WriteBytes(BinaryWriter bw, ref StringTable stringTable)
        {
            base.WriteHeader(bw, XNodeType.Page);

            bw.Write(this.Name ?? "null");
        }
    }
}
