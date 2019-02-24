using System;
using System.Collections.Generic;
using System.IO;

namespace DialogML.XNodes
{
    public class XNodeLog : XmlNode
    {
        public String Filter;
        public String Text;
        
        public void OnProcessElement(ScriptIds ids, string name, string value)
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
            bw.Write((ushort)XNodeType.Log);
            bw.Write((byte)1);    // Version Major
            bw.Write((byte)0);    // Version Minor
            bw.Write((ushort)(Children?.Count ?? 0));

            bw.Write(this.Id.ToByteArray() ?? Guid.Empty.ToByteArray());
            
            bw.Write(this.Filter ?? "");
            bw.Write(this.Text ?? "");

            //st.AddString(this.Id, this.Text);
        }
    }
}
