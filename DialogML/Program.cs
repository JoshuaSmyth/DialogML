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


        static PostParser postParser = new PostParser();

        static ScriptEngine scriptEngine; // Should script engine have it's own ScriptEngineData?

        public static void CompileProgram(List<string> files)
        {
            // TODO Return a gossipscript program?
            // TODO Create a compiler class
            throw new NotImplementedException();
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

                postParser.AddOrUpdateScript(root);

                var referenceTable = postParser.GetReferencesTable();

                // TODO Pass in references table (possibly needs to be a context at some stage)
                var bytes = bParser.SerializeXTree(root, ref stringTable, ref referenceTable);

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

            // TODO Parse set of scripts
            var scripts = new List<string>()
            {
                "Scripts/callpage.xml",
                "Scripts/callscript.xml",
                "Scripts/coroutine1.xml",
                "Scripts/dialog_druids_sample"
            };

            //CompileProgram(scripts);

            LoadScript("Scripts/callpage.xml");
            LoadAndRunScript("Scripts/callscript.xml");
            
            /*
            LoadAndRunScript("Scripts/coroutine1.xml");
            LoadAndRunScript("Scripts/dialog_druids_sample.xml");

            Console.WriteLine("** Second Run **");
            
            LoadAndRunScript("Scripts/dialog_druids_sample.xml");
            */    
        }
    }
}