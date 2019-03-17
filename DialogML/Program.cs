using DialogML.RNodes;
using DialogML.VM;
using System;
using System.Collections.Generic;
using System.IO;

namespace DialogML
{
    class ScriptEngineData
    {
        public OnlyIfTable OnlyIfTable = new OnlyIfTable();
        public RuntimeReferencesTable ReferencesTable = new RuntimeReferencesTable();
        public StringTable StringTable = new StringTable();
    }

    class Program
    {
        static Dictionary<String, CompiledScript> ScriptBank = new Dictionary<String, CompiledScript>();

        static ScriptEngineData ScriptEngineData = new ScriptEngineData();

        static ScriptEngine scriptEngine; // Should script engine have it's own ScriptEngineData?

        public static void LoadAndRunScript(string filename)
        {
            // Check if script is in the compiled script cache
            if(ScriptBank.ContainsKey(filename))
            {
                CompiledScript script = ScriptBank[filename];
                RunScript(scriptEngine, script);
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
                    
                    ScriptEngineData.StringTable.Deserialise(strings);
                    scriptFile.Deserialise(bytes);
                    
                    Console.WriteLine();
                    ScriptBank.Add(filename, scriptFile);
                    RunScript(scriptEngine, scriptFile);
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
        }

        private static void RunScript(ScriptEngine scriptEngine, CompiledScript script)
        {
            scriptEngine.StartScript(script);
            AdvanceType rv = AdvanceType.Unknown;
            while(rv != AdvanceType.Finished)
            {
                rv = scriptEngine.Update();
            }
        }

        static void Main(string[] args)
        {
            scriptEngine = new ScriptEngine(ScriptEngineData.ReferencesTable, ScriptEngineData.StringTable, ScriptEngineData.OnlyIfTable);

            LoadAndRunScript("Scripts/coroutine1.xml");

            // TODO LoadScript()
            // TODO Runscript()

            LoadAndRunScript("Scripts/dialog_druids_sample.xml");

            //Console.WriteLine();
            //Console.WriteLine("** Second Run **");
            //Console.WriteLine();

            LoadAndRunScript("Scripts/dialog_druids_sample.xml");
        }
    }
}