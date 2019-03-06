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

        public override void WriteBytes(BinaryWriter bw, ref StringTable st)
        {
            base.WriteHeader(bw, XNodeType.Wait);
            bw.Write(Milliseconds);
        }
    }
}
