using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    public class XNodeSelect : XmlNode
    {
        public bool RemoveOnSelect;
        public void OnProcessElement(string name, string value)
        {
            var loweredName = name.ToLower();
            if (loweredName == "remove-on-select")
            {
                var loweredValue = value.ToLower();
                if (loweredValue == "true")
                {
                    RemoveOnSelect = true;
                }
            }
        }

        public override void WriteBytes(BinaryWriter bw, ref StringTable st)
        {
            bw.Write((ushort)XNodeType.Select);
            bw.Write((byte)1);    // Version Major
            bw.Write((byte)0);    // Version Minor
            bw.Write((ushort)(Children?.Count ?? 0));

            bw.Write(RemoveOnSelect);
        }
    }
}
