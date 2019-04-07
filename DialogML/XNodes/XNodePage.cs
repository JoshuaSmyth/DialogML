using System;
using System.IO;

namespace DialogML.XNodes
{
    class XNodePage : XmlNode
    {
        public override void WriteBytes(CompileContext ctx)
        //public override void WriteBytes(BinaryWriter bw, string filename, ref StringTable stringTable, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(ctx.bw, XNodeType.Page);

            ctx.bw.Write(this.Name ?? "null");
        }
    }
}
