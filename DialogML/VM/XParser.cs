using DialogML.XNodes;
using System;

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
                case "log":
                    {
                        var node = new XNodeLog();
                        foreach(var a in element.Attributes())
                        {
                            node.OnProcessElement(ids, a.Name.ToString(), a.Value);
                        }

                        root.Children.Add(node);
                        newRoot = node;
                        break;
                    }
                case "case-false":
                    {
                        var node = new XNodeCaseFalse();
                        foreach(var a in element.Attributes())
                        {
                            node.OnProcessElement(ids, a.Name.ToString(), a.Value);
                        }

                        root.Children.Add(node);
                        newRoot = node;
                        break;
                    }
                case "case-true":
                    {
                        var node = new XNodeCaseTrue();
                        foreach(var a in element.Attributes())
                        {
                            node.OnProcessElement(ids, a.Name.ToString(), a.Value);
                        }

                        root.Children.Add(node);
                        newRoot = node;
                        break;
                    }
                case "if":
                    {
                        var node = new XNodeIf();
                        foreach(var a in element.Attributes())
                        {
                            node.OnProcessElement(ids, a.Name.ToString(), a.Value);
                        }

                        root.Children.Add(node);
                        newRoot = node;
                        break;
                    }
                case "option-exit":
                    {
                        var node = new XNodeOptionExit();
                        foreach(var a in element.Attributes())
                        {
                            node.OnProcessElement(ids, a.Name.ToString(), a.Value);
                        }

                        root.Children.Add(node);
                        newRoot = node;
                        break;
                    }
                case "option":
                    {
                        var node = new XNodeOption();
                        foreach(var a in element.Attributes())
                        {
                            node.OnProcessElement(ids, a.Name.ToString(), a.Value);
                        }

                        root.Children.Add(node);
                        newRoot = node;
                        break;
                    }
                case "select":
                    {
                        var node = new XNodeSelect();
                        foreach(var a in element.Attributes())
                        {
                            node.OnProcessElement(a.Name.ToString(), a.Value);
                        }

                        root.Children.Add(node);
                        newRoot = node;
                        break;
                    }
                case "say":
                    {
                        var node = new XNodeSay();
                        foreach(var a in element.Attributes())
                        {
                            node.OnProcessElement(ids, a.Name.ToString(), a.Value);
                        }

                        root.Children.Add(node);
                        newRoot = node;
                        break;
                    }
                case "goto-page":
                    {
                        var node = new XNodeGotoPage();
                        foreach(var a in element.Attributes())
                        {
                            var attributeName = a.Name.ToString().ToLower();
                            switch(attributeName)
                            {
                                case "name":
                                    node.PageName = a.Value;
                                    break;
                                default:
                                    ThrowInvalidAttributeError(elementName, attributeName);
                                    break;
                            }
                        }
                        break;
                    }
                case "print":
                    {
                        var node = new XNodePrint();
                        foreach(var a in element.Attributes())
                        {
                            var attributeName = a.Name.ToString().ToLower();
                            switch(attributeName)
                            {
                                case "text":
                                    node.Text = a.Value;
                                    break;
                                default:
                                    ThrowInvalidAttributeError(elementName, attributeName);
                                    break;
                            }
                        }

                        root.Children.Add(node);
                        break;
                    }
                case "exit":
                    {
                        root.Children.Add(new XNodeExit());
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
                case "define-global-flag":
                    {
                        Guid id = Guid.NewGuid();
                        String name = null;
                        foreach(var a in element.Attributes())
                        {
                            var attributeName = a.Name.ToString().ToLower();
                            switch(attributeName)
                            {
                                case "id":
                                    {
                                        id = new Guid();    // Read This;
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

                        newRoot = new XDefineVar() { Id = id, Name = name, VarType = VariableType.GlobalFlag };
                        root.Children.Add(newRoot);
                        break;
                    }

                case "define-local-var":
                    {
                        Guid id = Guid.NewGuid();
                        String name = null;
                        foreach(var a in element.Attributes())
                        {
                            var attributeName = a.Name.ToString().ToLower();
                            switch(attributeName)
                            {
                                case "id":
                                    {
                                        id = new Guid();    // Read This;
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

                        newRoot = new XDefineVar() { Id = id, Name = name, VarType = VariableType.LocalVar };
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

        private static void ThrowInvalidAttributeError(string elementName, string attributeName)
        {
            // TODO Make custom exception
            throw new Exception("Unknown Attribute:" + attributeName + " on element:" + elementName);
        }
    }
}
