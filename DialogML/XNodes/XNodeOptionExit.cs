using System;
using System.IO;

namespace DialogML.XNodes
{
    public class XNodeOptionExit : XmlNode
    {
        public string Text;
        public override void OnProcessElement(ScriptIds ids, string name, string value)
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

        public override void WriteBytes(BinaryWriter bw, ref StringTable st, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(bw, XNodeType.OptionExit);

            st.AddString(this.Id, this.Text);
        }
    }
}
