using System.IO;
using System.Xml;

namespace DialogML.XNodes
{
    class XNodeSet : XmlNode
    {
        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            // TODO
        }

        public override void WriteBytes(BinaryWriter bw, string filename, ref StringTable stringTable, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(bw, XNodeType.Set);

            bw.Write(this.Name ?? "null");
        }
    }
}