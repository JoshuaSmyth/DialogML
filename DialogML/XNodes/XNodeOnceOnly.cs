using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    public enum OnceOnlyConfig : byte
    {
        NoChildren = 0,
        SingleChildTrue = 1,
        SingleChildFalse = 2,
        TwinChildrenTrueFalse = 3,
        TwinChildrenFalseTrue = 4
    }

    public class XNodeOnceOnly : XmlNode
    {
        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            // Should this id stuff be handled already?
            var loweredName = name.ToLower();
            if(loweredName == "id")
            {
                Id = ids.GetGuidByIndex(value);
            }
        }

        public override void WriteBytes(BinaryWriter bw, ref StringTable stringTable) // TODO Pass string table
        {
            this.WriteHeader(bw, XNodeType.OnceOnly);

            // TODO Analyze children
            bw.Write((byte)OnceOnlyConfig.SingleChildFalse);
        }
    }
}
