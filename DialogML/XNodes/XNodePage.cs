using System;
using System.IO;

namespace DialogML.XNodes
{
    class XNodePage : XmlNode
    {
        public override void WriteBytes(BinaryWriter bw, string filename, ref StringTable stringTable, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(bw, XNodeType.Page);

            bw.Write(this.Name ?? "null");
        }
    }
}
