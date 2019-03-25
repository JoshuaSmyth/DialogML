using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    class XNodeCallScript : XmlNode
    {
        String TargetScript;
        String TargetPage;
        Guid PageId;
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
            if(loweredName == "script")
            {
                TargetScript = value;
            }
            if (loweredName == "page")
            {
                TargetPage = value;
            }
        }

        public override void WriteBytes(BinaryWriter bw, string scriptFilename, ref StringTable st, ref ReferencesTable referencesTable)
        {
            var page = referencesTable.GetPageRecord(this.TargetScript, this.TargetPage);
            if (page==null)
            {
                throw new Exception("Could not find page");
            }

            // Look up page name to get the id
            base.WriteHeader(bw, XNodeType.CallScript);
       //     bw.Write(this.TargetScript);
      //      bw.Write(this.TargetPage);
            bw.Write(page.PageId.ToByteArray());
        }
    }
}
