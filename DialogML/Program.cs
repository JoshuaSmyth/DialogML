using DialogML.RNodes;
using DialogML.VM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

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

        static PostParser postParser = new PostParser();

        static ScriptEngine scriptEngine;

        public static void CompileProgram(List<string> files)
        {
          
            // TODO Ids needs to be stored in the same file
            // So that file system operations are simplified.

            // Step 1: Assign Ids to the XML
            var preparser = new Preparser();
            var lstScriptIds = new List<ScriptIds>();
            foreach(var filename in files)
            {
                preparser.AssignIds(filename);

                // Read Ids into table
                var idsFile = Path.ChangeExtension(filename, "ids");
                var ids = File.ReadAllText(idsFile);

                var scriptIds = new ScriptIds();
                scriptIds.ParseText(ids);
                lstScriptIds.Add(scriptIds);
            }

            // Create the XML Tree Structure for each script
            var xParser = new XParser();
            var refParser = new PostParser();

            List<DialogML.XNodes.XmlNode> ProgramTrees = new List<XNodes.XmlNode>();
            var i = 0;
            foreach(var filename in files)
            {
                var xml = File.ReadAllText(filename);

                var scriptIds = lstScriptIds[i];
                var result = xParser.Process(scriptIds, xml);
                var root = result.Children[0];
                refParser.AddOrUpdateScript(root, filename);

                ProgramTrees.Add(root);
                i++;
            }

            // Compile into program
            var stringTable = new StringTable();
            i = 0;
            foreach(var root in ProgramTrees)
            {
                var fileName = files[i];
                var bParser = new BinarySerialiser();
                var referenceTable = refParser.GetReferencesTable();
                var bytes = bParser.SerializeXTree(root, fileName, ref stringTable, ref referenceTable);

                var script = new CompiledScript();
                script.Deserialise(bytes);

                ScriptEngineData.ReferencesTable.AddOrUpdateScript(script);
                ScriptBank.Add(fileName, script);
                i++;
            }

            // Assign all the strings to the engine
            var strings = stringTable.Serialise();
            ScriptEngineData.StringTable.Deserialise(strings);

        }

        public static void LoadScript(string filename)
        {
            if(File.Exists(filename))
            {
                // Run the preparser
                var xParser = new XParser();
                var bParser = new BinarySerialiser();
                var preparser = new Preparser();

                var stringTable = new StringTable();
                
                var idsFile = Path.ChangeExtension(filename, "ids");
                preparser.AssignIds(filename);

                var ids = File.ReadAllText(idsFile);

                var scriptIds = new ScriptIds();
                scriptIds.ParseText(ids);
                var scriptIdBytes = scriptIds.SerializeBytes();

                var xml = File.ReadAllText(filename);

                var result = xParser.Process(scriptIds, xml);
                var root = result.Children[0];

                postParser.AddOrUpdateScript(root, filename);

                var referenceTable = postParser.GetReferencesTable();
                
                var bytes = bParser.SerializeXTree(root, filename, ref stringTable, ref referenceTable);

                Console.WriteLine("ScriptIds Size:" + scriptIdBytes.Length);
                Console.WriteLine("Script Size:" + bytes.Length);

                var strings = stringTable.Serialise();
                ScriptEngineData.StringTable.Deserialise(strings);
                var script = new CompiledScript();
                script.Deserialise(bytes);

                Console.WriteLine("Loaded Script: " + filename);

                // TODO Add to the references
                
                
                ScriptEngineData.ReferencesTable.AddOrUpdateScript(script);
                ScriptBank.Add(filename, script);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

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
                LoadScript(filename);
                CompiledScript script = ScriptBank[filename];
                RunScript(scriptEngine, script);
            }
        }

        private static void RunScript(String filename)
        {
            CompiledScript script = ScriptBank[filename];
            RunScript(scriptEngine, script);
        }

        private static void RunScript(ScriptEngine scriptEngine, CompiledScript script)
        {
            Console.WriteLine();
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

            // Parse set of scripts
            var scripts = new List<string>()
            {
                "Scripts/callpage.xml",
                "Scripts/callscript.xml",
                "Scripts/concurrent1.xml",
                "Scripts/dialog_druids_sample.xml",
                "Scripts/wait.xml",
                "Scripts/condition1.xml"
            };

            var sw = Stopwatch.StartNew();
                CompileProgram(scripts);
            sw.Stop();
            Console.WriteLine(String.Format("Time Taken to compile scripts:{0}ms", sw.ElapsedMilliseconds));

            RunScript("Scripts/callscript.xml");
        }
    }
}