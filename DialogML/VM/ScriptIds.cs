using System;
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

        public void Parse(String text)
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
            Parse(File.ReadAllText(idsFilename));
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
