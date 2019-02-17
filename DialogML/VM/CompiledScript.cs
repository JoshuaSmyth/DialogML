﻿using DialogML.DNodes;
using DialogML.XNodes;
using System;
using System.Collections.Generic;
using System.IO;

namespace DialogML.RNodes
{
    public struct NodeHeader
    {
        public ushort XNodeType;
        public ushort VersionMajor;
        public ushort VersionMinor;
        public int ChildrenCount;
    }

    public class CompiledScript
    {
        public Guid Id;
        public String Name;
        public RNode Root;

        private void DeserialiseTree(RNode root, BinaryReader br)
        {
            // Read the header
            var header = new NodeHeader()
            {
                XNodeType = br.ReadUInt16(),
                VersionMajor = br.ReadByte(),
                VersionMinor = br.ReadByte(),
                ChildrenCount = br.ReadUInt16()
            };

            // Decode Type
            RNode newRoot = null;
            var xnodeType = (XNodeType)header.XNodeType;
            switch(xnodeType)
            {
                case XNodeType.OptionExit:
                    {
                        var id = new Guid(br.ReadBytes(16));
                        newRoot = new RNodeOptionExit(id);
                        break;
                    }
                case XNodeType.Option:
                    {
                        var id = new Guid(br.ReadBytes(16));
                        newRoot = new RNodeOption(id);
                        break;
                    }
                case XNodeType.Select:
                    {
                        var removeOnSelect = br.ReadBoolean();
                        newRoot = new RNodeSelect(removeOnSelect);
                        break;
                    }
                case XNodeType.Say:
                    {
                        var id = new Guid(br.ReadBytes(16));
                        var sayPosition = (SayPosition)(br.ReadByte());
                        var actorId = br.ReadString();

                        newRoot = new RNodeSay(id, sayPosition, actorId);
                        break;
                    }
                case XNodeType.Page:
                {
                    var id = new Guid(br.ReadBytes(16));
                    var name = br.ReadString();

                    newRoot = new RNodePage(id, name);
                    break;
                }
                case XNodeType.Script:
                {
                    var id = new Guid(br.ReadBytes(16));
                    var name = br.ReadString();

                    newRoot = new RNodeScript(id, name);
                    break;
                }
                default:
                    throw new Exception("Unknown node type:" + xnodeType);
            }

            root.Children.Add(newRoot);
            // Decode children
            for(int i=0;i<header.ChildrenCount;i++)
            {
                DeserialiseTree(newRoot, br);
            }
        }

        public void Deserialise(byte[] bytes)
        {
            Root = new RNode();
            using(var ms = new MemoryStream(bytes))
            {
                using(var br = new BinaryReader(ms))
                {

                    var magicString = br.ReadString();
                    if (magicString != "DMLB")
                    {
                        throw new Exception("InvalidHeader");
                    }
                    var versionMajor = br.ReadUInt16();
                    var versionMinor = br.ReadUInt16();

                    // Decode the root node
                    DeserialiseTree(Root, br);
                }
            }
        }
    }


}
