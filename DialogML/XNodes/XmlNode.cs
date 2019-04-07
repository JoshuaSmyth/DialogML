using DialogML.VM;
using ExpressionParser;
using System;
using System.Collections.Generic;
using System.IO;

namespace DialogML.XNodes
{
    public class CompileContext
    {
        public BinaryWriter bw;
        public String scriptFilename;
        public StringTable stringTable;
        public ReferencesTable referencesTable;
        public RpnCompiler rpnCompiler;
    }


    public class XmlNode
    {
        public String Name;
        public Guid Id;
        public AdvanceType Update(ScriptApi api) { return AdvanceType.Next; }
        public List<XmlNode> Children = new List<XmlNode>();

        public virtual void OnProcessElement(ScriptIds ids, string name, string value)
        {
            // Implement Me!
            throw new Exception("Implement Me!");
        }

        public virtual void LookupReferences(RuntimeReferencesTable table)
        {

        }

        public virtual void WriteBytes(CompileContext ctx)
        //public virtual void WriteBytes(BinaryWriter bw, string scriptFileName, ref StringTable stringTable, ref ReferencesTable referencesTable)
        {
            ctx.bw.Write((ushort)XNodeType.Unknown);
            ctx.bw.Write((byte)1);    // Version Major
            ctx.bw.Write((ushort)(Children?.Count ?? 0));

            ctx.bw.Write(this.Id.ToByteArray() ?? Guid.Empty.ToByteArray());
        }

        internal void WriteHeader(BinaryWriter bw, XNodeType select)
        {
            bw.Write((byte)select);
            bw.Write((byte)1);
            bw.Write((ushort)(Children?.Count ?? 0));
            bw.Write(this.Id.ToByteArray() ?? Guid.Empty.ToByteArray());
        }
    }
}
