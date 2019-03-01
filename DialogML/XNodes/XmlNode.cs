﻿using System;
using System.Collections.Generic;
using System.IO;

namespace DialogML.XNodes
{
    public class XmlNode
    {
        public String Name;
        public Guid Id;
        public AdvanceType Update(ScriptApi api) { return AdvanceType.Next; }
        public List<XmlNode> Children = new List<XmlNode>();

        public virtual void WriteBytes(BinaryWriter bw, ref StringTable stringTable) // TODO Pass string table
        {
            bw.Write((ushort)XNodeType.Unknown);
            bw.Write((byte)1);    // Version Major
            //bw.Write((byte)0);    // Version Minor
            bw.Write((ushort)(Children?.Count ?? 0));

            bw.Write(this.Id.ToByteArray() ?? Guid.Empty.ToByteArray());
            //bw.Write(this.Name ?? "null");
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
