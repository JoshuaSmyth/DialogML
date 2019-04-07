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
            if (loweredName == "target" || loweredName == "page" || loweredName == "name")
            {
                TargetPage = value;
            }
        }


        public override void WriteBytes(CompileContext ctx)
        //public override void WriteBytes(BinaryWriter bw, String scriptFilename, ref StringTable st, ref ReferencesTable referencesTable)
        {
            var page = ctx.referencesTable.GetPageRecord(ctx.scriptFilename, this.TargetPage);
            if (page == null)
            {
                throw new Exception("Could not find page");
            }
            base.WriteHeader(ctx.bw, XNodeType.CallPage);
       //     bw.Write(this.TargetPage);
            ctx.bw.Write(page.PageId.ToByteArray());
        }
    }
}
