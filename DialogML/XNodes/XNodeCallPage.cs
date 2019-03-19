﻿using System;
using System.IO;

namespace DialogML.XNodes
{
    class XNodeCallPage : XmlNode
    {
        String TargetPage;
        Guid PageId;
        public override void OnProcessElement(ScriptIds ids, string name, string value)
        {
            var loweredName = name.ToLower();
            if(loweredName == "id")
            {
                if(value == "{guid}")
                {
                    Id = Guid.NewGuid();
                }
                else
                {
                    Id = ids.GetGuidByIndex(value); //Guid.Parse(value);
                }
            }
            if (loweredName == "target" || loweredName == "page")
            {
                TargetPage = value;
            }
        }

        public override void WriteBytes(BinaryWriter bw, ref StringTable st)
        {
            // Look up page name to get the id

            base.WriteHeader(bw, XNodeType.CallPage);
            bw.Write(this.TargetPage);
        }
    }
}
