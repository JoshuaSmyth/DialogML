using DialogML.XNodes;
using System;
using System.Xml.Linq;

namespace DialogML
{
    public class XParser
    {
        public XmlNode Process(ScriptIds ids, String xml)
        {
            var xDocument = System.Xml.Linq.XDocument.Parse(xml);

            var rv = new XmlNode();
            Process(ids, xDocument.Root, rv);
            return rv;
        }
        
        // TODO Actually write out the grammer rules
        public void Process(ScriptIds ids, System.Xml.Linq.XElement element, XmlNode root)
        {
            XmlNode newRoot = null;

            var elementName = element.Name.ToString().ToLower();
            switch(elementName)
            {
                case "wait":
                    {
                        var node = new XNodeWait();
                        newRoot = InitNode(ids, element, root, node);
                        break;
                    }

                case "return":
                    {
                        var node = new XNodeReturn();
                        newRoot = InitNode(ids, element, root, node);
                        break;
                    }

                case "once-only":
                    {
                        var node = new XNodeOnceOnly();
                        newRoot = InitNode(ids, element, root, node);
                        break;
                    }
                case "log":
                    {
                        var node = new XNodeLog();
                        newRoot = InitNode(ids, element, root, node);
                        break;
                    }
                case "case-false":
                    {
                        var node = new XNodeCaseFalse();
                        newRoot = InitNode(ids, element, root, node);
                        break;
                    }
                case "case-true":
                    {
                        var node = new XNodeCaseTrue();
                        newRoot = InitNode(ids, element, root, node);
                        break;
                    }
                case "if":
                    {
                        var node = new XNodeIf();
                        newRoot = InitNode(ids, element, root, node);
                        break;
                    }
                case "option-exit":
                    {
                        var node = new XNodeOptionExit();
                        newRoot = InitNode(ids, element, root, node);
                        break;
                    }
                case "option":
                    {
                        var node = new XNodeOption();
                        newRoot = InitNode(ids, element, root, node);
                        break;
                    }
                case "select":
                    {
                        var node = new XNodeSelect();
                        newRoot = InitNode(ids, element, root, node);
                        break;
                    }
                case "say":
                    {
                        var node = new XNodeSay();
                        newRoot = InitNode(ids, element, root, node);
                        break;
                    }
                case "page":
                    {
                        Guid id = Guid.Empty;
                        String name = null;
                        foreach(var a in element.Attributes())
                        {
                            var attributeName = a.Name.ToString().ToLower();
                            switch(attributeName)
                            {
                                case "id":
                                    {
                                        id = ids.GetGuidByIndex(a.Value);
                                        //id = new Guid(a.Value);
                                        break;
                                    }
                                case "name":
                                    {
                                        name = a.Value;
                                        break;
                                    }
                                default:
                                    {
                                        ThrowInvalidAttributeError(elementName, attributeName);
                                        break;
                                    }
                            }
                        }

                        newRoot = new XNodePage() { Id = id, Name = name };
                        root.Children.Add(newRoot);
                        break;
                    }
                
                case "script":
                    {
                        Guid id = Guid.Empty;
                        String name = null;

                        // Read the attributes
                        foreach(var a in element.Attributes())
                        {
                            var attributeName = a.Name.ToString().ToLower();
                            switch(attributeName)
                            {
                                case "id":
                                    {
                                        if(a.Value == "{guid}")
                                        {
                                            id = new Guid();
                                        }
                                        else
                                        {
                                            id = ids.GetGuidByIndex(a.Value); //new Guid(a.Value);
                                        }
                                        break;
                                    }
                                case "name":
                                    {
                                        name = a.Value;
                                        break;
                                    }
                                default:
                                    {
                                        Console.WriteLine("unknown attribute:" + attributeName);
                                        break;
                                    }
                            }
                        }

                        newRoot = new XScript() { Id = id, Name = name };
                        root.Children.Add(newRoot);
                        break;
                    }

                default:
                    {

                        throw new Exception("unknown node: " + element.Name);
                    }
            }

            // Recurse
            if(element.HasElements)
            {
                foreach(var child in element.Elements())
                {
                    Process(ids, child, newRoot);
                }
            }
        }

        private static XmlNode InitNode(ScriptIds ids, XElement element, XmlNode currentRoot, XmlNode newNode)
        {
            XmlNode newRoot;
            foreach(var a in element.Attributes())
            {
                newNode.OnProcessElement(ids, a.Name.ToString(), a.Value);
            }

            currentRoot.Children.Add(newNode);
            newRoot = newNode;
            return newRoot;
        }

        private static void ThrowInvalidAttributeError(string elementName, string attributeName)
        {
            // TODO Make custom exception
            throw new Exception("Unknown Attribute:" + attributeName + " on element:" + elementName);
        }
    }
}
