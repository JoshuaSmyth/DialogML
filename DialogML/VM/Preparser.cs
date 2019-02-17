using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DialogML.VM
{
    public class Preparser
    {
        public void Preparse(string filename)
        {
            var scriptIds = new ScriptIds();
            var idsFilename = Path.ChangeExtension(filename, "ids");
            if (File.Exists(idsFilename))
            {
                scriptIds.FromFile(idsFilename);
            }

            var text = File.ReadAllText(filename);
            var xDocument = XDocument.Parse(text);

            var modified = false;
            Process(xDocument.Root, scriptIds, ref modified);
            if (modified)
            {
                // Write back to file
                var output = xDocument.ToString();
                File.WriteAllText(filename, output);

                scriptIds.WriteTextFile(idsFilename);
            }
        }

        private void Process(XElement element, ScriptIds scriptIds, ref bool modified)
        {
            var attribute = element.Attribute("id");
            if (attribute == null)
            {
                // TODO Insert at end
                // https://stackoverflow.com/questions/17581098/insert-xattribute-to-existing-xelement-at-specified-position

                var key = scriptIds.NewEntry();
                attribute = new XAttribute("id", key);

                element.Add(attribute);
                modified = true;
            }
            else
            {
                var key = attribute.Value;
                if (!scriptIds.ContainsKey(key))
                {
                    throw new Exception("Entry not found");
                }

                // TODO Put the id element at the end
                // TODO Check the id value is in the scriptIds table
                modified = true;
            }

            if(element.HasElements)
            {
                foreach(var child in element.Elements())
                {
                    Process(child, scriptIds, ref modified);
                }
            }
        }
    }
}
