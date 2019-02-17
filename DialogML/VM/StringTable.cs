using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML
{
    public class StringTable
    {
        private Dictionary<Guid, string> Strings = new Dictionary<Guid, string>();

        public void AddString(Guid guid, String text)
        {
            Strings.Add(guid, text);
        }

        public String GetString(Guid guid)
        {
            return Strings[guid];
        }

        public byte[] Serialise()
        {
            using(var ms = new MemoryStream())
            {
                using(var bw = new BinaryWriter(ms))
                {
                    bw.Write("DMLS");
                    bw.Write((ushort)1);        // Version Major
                    bw.Write((ushort)0);        // Version Minor
                    bw.Write((Int32)Strings.Count);
                    foreach(var kvp in Strings)
                    {
                        bw.Write(kvp.Key.ToByteArray());
                        bw.Write(kvp.Value);
                    }
                    return ms.ToArray();
                }
            }
        }

        public void Deserialise(byte[] bytes)
        {
            using(var ms = new MemoryStream(bytes))
            {
                using(var br = new BinaryReader(ms))
                {
                    // TODO Use 4 bytes for DMLS instead of 5

                    /*
                    var b1 = br.ReadByte();
                    if (b1 != 4)
                    {
                        throw new Exception("InvalidHeader");
                    }
                    */
                    var chars = br.ReadString();
                    if (chars != "DMLS")
                    {
                        throw new Exception("InvalidHeader");
                    }

                    var versionMajor = br.ReadUInt16();
                    if (versionMajor != 1)
                    {
                        throw new Exception("InvalidHeader");
                    }

                    var versionMinor = br.ReadUInt16();
                    if(versionMinor != 0)
                    {
                        throw new Exception("InvalidHeader");
                    }

                    var stringCount = br.ReadInt32();
                    var newTable = new Dictionary<Guid, string>(stringCount);
                    for(int i=0;i<stringCount;i++)
                    {
                        var id = new Guid(br.ReadBytes(16));
                        var s = br.ReadString();
                        newTable.Add(id, s);
                    }

                    Strings.Clear();
                    Strings = newTable;
                }
            }
        }
    }
}
