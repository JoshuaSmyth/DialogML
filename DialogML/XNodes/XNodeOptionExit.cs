using System;
using System.IO;

namespace DialogML.XNodes
{
    public class XNodeOptionExit : XmlNode
    {
        public string Text;
        public void OnProcessElement(ScriptIds ids, string name, string value)
        {
            var loweredName = name.ToLower();
            if(loweredName == "text")
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
            bw.Write((ushort)XNodeType.OptionExit);
            bw.Write((byte)1);    // Version Major
            bw.Write((byte)0);    // Version Minor
            bw.Write((ushort)(Children?.Count ?? 0));

            bw.Write(this.Id.ToByteArray() ?? Guid.Empty.ToByteArray());
            st.AddString(this.Id, this.Text);
        }
    }
}
