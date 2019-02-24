﻿using DialogML.RNodes;
using DialogML.VM;
using DialogML.XNodes;
using System;
using System.Collections.Generic;
using System.IO;

namespace DialogML
{
    public class Expression
    {
        String expression;  // TODO Make RPN Tokens
    }
    
    public class XNodePrint : XmlNode
    {
        public String Text;
    }

    class XNodeSetGlobalFlag : XmlNode
    {
        public string value;                       // Could be expression
    }

    class XNodeOnlyIf : XmlNode
    {
        public Expression Expression;

        public bool InvertResult;                  // Becomes Only-If-Not

        public AdvanceType Update(ScriptApi api)
        {
            if(api.EvaluateExpression(Expression))
            {
                return AdvanceType.FirstChild;
            }
            else
            {
                return AdvanceType.Next;
            }
        }
    }

    /*
    class XNodeIf : XmlNode
    {
        public Expression Expression;
        public bool InvertResult;

        public AdvanceType Update(ScriptApi api)
        {
            // TODO Must have only two children (Enforce at compilation stage)
            if(api.EvaluateExpression(Expression))
            {
                // Or return childId?
                return AdvanceType.FirstChild;
            }
            else
            {
                return AdvanceType.SecondChild;
            }
        }
    }*/

    class XNodeGotoPage : XmlNode
    {
        public string PageName;
        Guid Id;            // Pushes this id
    }

    class XNodeOptions : XmlNode
    {
        // List of options
    }

    class XNodeChoice : XmlNode
    {
        public String Text;
        public Guid GotoPage;
        public Expression OnlyIf;
    }

    class XNodeExit : XmlNode
    {
        // No Parameters
    }




    class XDefineVar : XmlNode
    {
        public VariableType VarType;
        public String InitalValue;
    }

    class XScriptVariable
    {
        VariableType variableType;
        public string Name;
        public Guid Id;
        public string value;            // All types are really strings shocking!
    }


    class Program
    {
        public static void LoadAndRunScript(string filename)
        {
            if(File.Exists(filename))
            {
                // Run the preparser
                var xParser = new XParser();
                var bParser = new BinarySerialiser();
                var preparser = new Preparser();

                var stringTable = new StringTable();
                var scriptFile = new CompiledScript();

                var idsFile = Path.ChangeExtension(filename, "ids");
                preparser.Preparse(filename);
                
                var scriptIds = new ScriptIds();
                var ids = File.ReadAllText(idsFile);
                scriptIds.Parse(ids);

                var xml = File.ReadAllText(filename);

                var result = xParser.Process(scriptIds, xml);
                var root = result.Children[0];


                var bytes = bParser.SerializeXTree(root, ref stringTable);

                var strings = stringTable.Serialise();

                stringTable.Deserialise(strings);
                scriptFile.Deserialise(bytes);

                // Run Engine
                Console.WriteLine();
                var scriptEngine = new ScriptEngine(stringTable);
                scriptEngine.StartScript(scriptFile);

                AdvanceType rv = AdvanceType.Unknown;
                while(rv != AdvanceType.Finished)
                {
                    rv = scriptEngine.Update();
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }


        static void Main(string[] args)
        {

            LoadAndRunScript("c:/test/TestScripts/condition1.xml");

            //    LoadAndRunScript("c:/test/TestScripts/dialog_druids_sample_noids.xml");

        }
    }
}
