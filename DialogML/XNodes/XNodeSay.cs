using System;
using System.Collections.Generic;
using System.IO;

namespace DialogML.XNodes
{
    public enum SayPosition : byte
    {
        None = 0,
        Left = 1,
        Right = 2
    }

    public class XNodeSay : XmlNode
    {
        public String ActorId;
        public String Text;
        public SayPosition Position;

        public void OnProcessElement(ScriptIds ids, string name, string value)
        {
            var loweredName = name.ToLower();

            if (loweredName == "actor-id")
            {
                ActorId = value;
            }
            else if (loweredName == "position")
            {
                var loweredValue = value.ToLower();
                if (loweredValue == "left")
                {
                    Position = SayPosition.Left;
                }
                else if(loweredValue == "right")
                {
                    Position = SayPosition.Right;
                }
            }
            else if (loweredName == "text")
            {
                Text = value;
            }
            else if (loweredName == "id")
            {
                if (value == "{guid}")
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
            bw.Write((ushort)XNodeType.Say);
            bw.Write((byte)1);    // Version Major
            bw.Write((byte)0);    // Version Minor
            bw.Write((ushort)(Children?.Count ?? 0));

            bw.Write(this.Id.ToByteArray() ?? Guid.Empty.ToByteArray());

            bw.Write((byte)this.Position);
            bw.Write(this.ActorId ?? "");

            st.AddString(this.Id, this.Text);
        }
    }
}
