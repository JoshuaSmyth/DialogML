using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    public class XNodeSelect : XmlNode
    {
        public bool RemoveOnSelect;
        public bool Unique;
        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            var loweredName = name.ToLower();
            if (loweredName == "remove-on-select")
            {
                var loweredValue = value.ToLower();
                if (loweredValue == "true")
                {
                    RemoveOnSelect = true;
                }
            }
            if (loweredName == "unique")
            {
                var loweredValue = value.ToLower();
                if(loweredValue == "true")
                {
                    Unique = true;
                }
            }
        }

        public override void WriteBytes(CompileContext ctx)
        //public override void WriteBytes(BinaryWriter bw, string filename, ref StringTable st, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(ctx.bw, XNodeType.Select);
            ctx.bw.Write(RemoveOnSelect);
            ctx.bw.Write(Unique);
        }
    }
}
