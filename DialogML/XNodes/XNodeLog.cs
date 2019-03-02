using System;
using System.IO;

namespace DialogML.XNodes
{
    public class XNodeLog : XmlNode
    {
        public String Filter;
        public String Text;
        
        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            var loweredName = name.ToLower();
            if(loweredName == "filter")
            {
                Filter = value;
            }
            else if(loweredName == "text")
            {
                Text = value;
            }
            else if(loweredName == "id")
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
            base.WriteHeader(bw, XNodeType.Log);

            var filterId = 0;
            if (this.Filter.ToLower() == "debug")
            {
                filterId = 1;
            }

            bw.Write((byte)filterId);
            bw.Write(this.Text ?? "");
        }
    }
}
