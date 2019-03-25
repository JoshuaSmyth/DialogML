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
        }

        public override void WriteBytes(BinaryWriter bw, string filename, ref StringTable st, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(bw, XNodeType.Select);
            bw.Write(RemoveOnSelect);
        }
    }
}
