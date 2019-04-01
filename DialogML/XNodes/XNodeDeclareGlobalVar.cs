﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.XNodes
{
    class XNodeDeclareGlobalVar : XmlNode
    {
        String Name;
        String Value;
        String Type;

        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            var loweredName = name.ToLower();
            if(loweredName == "name")
            {
                Name = value;
            }
            if(loweredName == "value")
            {
                Value = value;
            }
            if(loweredName == "type")
            {
                Type = value;
            }
        }

        public override void WriteBytes(BinaryWriter bw, string filename, ref StringTable stringTable, ref ReferencesTable referencesTable)
        {
            base.WriteHeader(bw, XNodeType.DeclareGlobalVar);

            bw.Write(this.Name ?? "null");
        }
    }
}
