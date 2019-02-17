using DialogML.XNodes;
using System;
using System.Collections.Generic;
using System.IO;

namespace DialogML
{
    public class BinarySerialiser
    {
        Dictionary<Guid, String> StringTable = new Dictionary<Guid, string>();

        public byte[] SerializeXTree(XmlNode root, ref StringTable st)
        {
            using(var ms = new MemoryStream())
            {
                using(var bw = new BinaryWriter(ms))
                {
                    // Header
                    bw.Write("DMLB");
                    bw.Write((ushort)1);        // Version Major
                    bw.Write((ushort)0);        // Version Minor
                    SerializeXTreeRecurse(root, bw, ref st);

                    return ms.ToArray();
                }
            }
        }
        
        private void SerializeXTreeRecurse(XmlNode root, BinaryWriter bw, ref StringTable st)
        {
            root.WriteBytes(bw, ref st);

            foreach(var child in root.Children)
            {
                SerializeXTreeRecurse(child, bw, ref st);
            }
        }
    }
}
