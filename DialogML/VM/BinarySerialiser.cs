using DialogML.XNodes;
using System;
using System.Collections.Generic;
using System.IO;

namespace DialogML
{
    public class BinarySerialiser
    {
        Dictionary<Guid, String> StringTable = new Dictionary<Guid, string>();

        public byte[] SerializeXTree(XmlNode root, string filename, ref StringTable st, ref ReferencesTable referencesTable)
        {
            using(var ms = new MemoryStream())
            {
                using(var bw = new BinaryWriter(ms))
                {
                    // Header
                    bw.Write("DMLB");
                    bw.Write((ushort)1);        // Version Major
                    bw.Write((ushort)0);        // Version Minor

                    var c = new CompileContext
                    {
                        referencesTable = referencesTable,
                        bw = bw,
                        stringTable = st,
                        scriptFilename = filename
                    };

                    SerializeXTreeRecurse(root, c);

                    return ms.ToArray();
                }
            }
        }
        
        private void SerializeXTreeRecurse(XmlNode root, CompileContext ctx)
        {
            root.WriteBytes(ctx);

            foreach(var child in root.Children)
            {
                SerializeXTreeRecurse(child, ctx);
            }
        }
    }
}
