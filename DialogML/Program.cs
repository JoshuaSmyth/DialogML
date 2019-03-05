﻿using DialogML.RNodes;
using DialogML.VM;
using System;
using System.Collections.Generic;
using System.IO;

namespace DialogML
{
    class Program
    {
        static Dictionary<String, CompiledScript> ScriptBank = new Dictionary<String, CompiledScript>();
        static Dictionary<String, StringTable> StringBank = new Dictionary<String, StringTable>();


        public static void LoadAndRunScript(string filename)
        {
            // Check if script is in the compiled script cache
            if(ScriptBank.ContainsKey(filename))
            {
                StringTable stringTable = StringBank[filename];
                CompiledScript script = ScriptBank[filename];

                var scriptEngine = new ScriptEngine(stringTable);
                scriptEngine.StartScript(script);

                AdvanceType rv = AdvanceType.Unknown;
                while(rv != AdvanceType.Finished)
                {
                    rv = scriptEngine.Update();
                }
            }
            else
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
                    scriptIds.ParseText(ids);
                    var scriptIdBytes = scriptIds.SerializeBytes();

                    var xml = File.ReadAllText(filename);

                    var result = xParser.Process(scriptIds, xml);
                    var root = result.Children[0];

                    var bytes = bParser.SerializeXTree(root, ref stringTable);

                    Console.WriteLine("ScriptIds Size:" + scriptIdBytes.Length);
                    Console.WriteLine("Script Size:" + bytes.Length);

                    var strings = stringTable.Serialise();

                    // TODO Write .strings file

                    stringTable.Deserialise(strings);
                    scriptFile.Deserialise(bytes);

                    // Run Engine
                    Console.WriteLine();
                    var scriptEngine = new ScriptEngine(stringTable);
                    scriptEngine.StartScript(scriptFile);

                    ScriptBank.Add(filename, scriptFile);
                    StringBank.Add(filename, stringTable);

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
        }
        
        static void Main(string[] args)
        {
            //LoadAndRunScript("Scripts/condition1.xml");

            LoadAndRunScript("Scripts/dialog_druids_sample.xml");

            Console.WriteLine();
            Console.WriteLine("** Second Run **");
            Console.WriteLine();

            LoadAndRunScript("Scripts/dialog_druids_sample.xml");

            // TODO Add ability to rerun scripts.
            // (Load and store them in some cache)
        }
    }
}