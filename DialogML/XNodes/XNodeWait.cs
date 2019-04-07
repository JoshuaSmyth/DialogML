using System;
using System.Collections.Generic;
using System.IO;

namespace DialogML.XNodes
{
    class XNodeWait : XmlNode
    {
        public Int32 Milliseconds;
        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            var loweredName = name.ToLower();
            if(loweredName == "time")
            {
                Milliseconds = Int32.Parse(value);
            }
        }

        public override void WriteBytes(CompileContext ctx)
        //public override void WriteBytes(BinaryWriter bw, string filename, ref StringTable st, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(ctx.bw, XNodeType.Wait);
            ctx.bw.Write(Milliseconds);
        }
    }
}
