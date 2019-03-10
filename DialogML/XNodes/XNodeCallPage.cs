using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    class XNodeCallPage : XmlNode
    {
        String PageName;
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
        }

        public override void WriteBytes(BinaryWriter bw, ref StringTable st)
        {
            //if (PageId == Guid.Empty)
            //{
            //    throw new Exception("Look up page id");
            //}

            // Look up page name to get the id

            base.WriteHeader(bw, XNodeType.CallPage);
        }
    }
}
