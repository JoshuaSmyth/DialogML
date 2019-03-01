﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML
{
    public class ScriptIds
    {
        Dictionary<string, Guid> Ids = new Dictionary<string, Guid>();

        public Guid GetGuidByIndex(string index)
        {
            return Ids[index];
        }

        public void ParseText(String text)
        {
            var items = text.Split(new char[] { '\r', '\n', ':' },StringSplitOptions.RemoveEmptyEntries);
            var len = items.Count();
            for(int i=0;i< len; i+=2)
            {
                var guid = Guid.Parse(items[i + 1]);
                Ids.Add(items[i], guid);
            }
        }

        internal void FromFile(string idsFilename)
        {
            ParseText(File.ReadAllText(idsFilename));
        }

        internal void WriteTextFile(string idsFilename)
        {
            var sb = new StringBuilder();
            foreach(var id in Ids)
            {
                sb.AppendLine(id.Key + ": " + id.Value);
            }
            File.WriteAllText(idsFilename, sb.ToString());
        }

        internal byte[] SerializeBytes()
        {
            using(var ms = new MemoryStream())
            {
                using(var bw = new BinaryWriter(ms))
                {
                    bw.Write((byte)42);
                    bw.Write((Int32)Ids.Count);
                    foreach(var id in Ids)
                    {
                        var hex = id.Key.Substring(1, id.Key.Length - 1);
                        var value = Convert.ToUInt32(hex, 16);
                        bw.Write(value);

                        bw.Write(id.Value.ToByteArray());
                    }

                    return ms.ToArray();
                }
            }
        }

        internal void WriteBinaryFile(string idsFilename)
        {

            throw new NotImplementedException();
        }

        internal string NewEntry()
        {
            lock (Ids)
            {
                var i = Ids.Count;
                i++;
                var key = "#" + i.ToString("X4");
                Ids.Add(key, Guid.NewGuid());
                return key;
            }
        }

        internal bool ContainsKey(string key)
        {
            return Ids.ContainsKey(key);
        }
    }
}
