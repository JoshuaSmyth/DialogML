using System;
using System.IO;

namespace DialogML.XNodes
{
    class XNodeCallPage : XmlNode
    {
        String TargetPage;
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
            if (loweredName == "target" || loweredName == "page")
            {
                TargetPage = value;
            }
        }

        public override void WriteBytes(BinaryWriter bw, ref StringTable st, ref ReferencesTable referencesTable)
        {
            // Look up page name to get the id
            // TODO Pass the script name or Id into WriteBytes method
            var page = referencesTable.GetPageRecord(Guid.Empty, this.TargetPage);

            base.WriteHeader(bw, XNodeType.CallPage);
            bw.Write(this.TargetPage);

            if(page == null)
            {
                bw.Write(Guid.Empty.ToByteArray());
            }
            else
            {
                bw.Write(page.PageId.ToByteArray());
            }
        }
    }
}
