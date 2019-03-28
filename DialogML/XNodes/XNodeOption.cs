using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    public class XNodeOption : XmlNode
    {
        public string Text;
        public bool RemoveOnSelect = false;
        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            var loweredName = name.ToLower();
            if(loweredName == "text")
            {
                Text = value;
            }
            else if (loweredName == "id")
            {
                if(value == "{guid}")
                {
                    Id = Guid.NewGuid();
                }
                else
                {
                    Id = ids.GetGuidByIndex(value); //Guid.Parse(value);
                }
            } else if (loweredName == "remove-on-select")
            {
                RemoveOnSelect = bool.Parse(value);
            }
        }

        public override void WriteBytes(BinaryWriter bw, string fileName, ref StringTable st, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(bw, XNodeType.Option);

            st.AddString(this.Id, this.Text);
            bw.Write(RemoveOnSelect);
            //bw.Write(this.Text ?? "");
        }
    }
}
