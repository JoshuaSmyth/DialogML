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
        public override void WriteBytes(CompileContext ctx)
        //public override void WriteBytes(BinaryWriter bw, String scriptFilename, ref StringTable st, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(ctx.bw, XNodeType.Script);

            ctx.bw.Write(this.Name ?? "Unnamed");
        }
    }
}
