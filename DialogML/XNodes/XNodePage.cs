using System;
using System.IO;

namespace DialogML.XNodes
{
    class XNodePage : XmlNode
    {
        public override void WriteBytes(BinaryWriter bw, ref StringTable stringTable)
        {
            bw.Write((ushort)XNodeType.Page);
            bw.Write((byte)1);    // Version Major
            bw.Write((byte)0);    // Version Minor
            bw.Write((ushort)(Children?.Count ?? 0));

            bw.Write(this.Id.ToByteArray() ?? Guid.Empty.ToByteArray());
            bw.Write(this.Name ?? "null");
        }
    }
}
