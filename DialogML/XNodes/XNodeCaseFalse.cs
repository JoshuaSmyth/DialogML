using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    public class XNodeCaseFalse : XmlNode
    {
        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            var loweredName = name.ToLower();
            if(loweredName == "id")
            {
                if(value == "{guid}")
                {
                    Id = Guid.NewGuid();
                }
                else
                {
                    Id = ids.GetGuidByIndex(value); //Guid.Parse(value);
                }
            }
        }

        public override void WriteBytes(BinaryWriter bw, string scriptFileName, ref StringTable st, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(bw, XNodeType.CaseFalse);
        }
    }
}
