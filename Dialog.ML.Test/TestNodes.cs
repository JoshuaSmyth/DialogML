using DialogML;
using DialogML.RNodes;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Dialog.ML.Test
{
    [TestFixture]
    public class TestNodes
    {

        [Test]
        public void Pass()
        {
            Assert.Pass();
        }

        [Test]
        public void ParseSimple_001()
        {
            var xml = @"
<script>
</script>
";
            var xParser = new XParser();
            var result = xParser.Process(new ScriptIds(), xml);

            Assert.NotNull(result);
        }

        [Test]
        public void ParseSimple_002()
        {
            var xml = @"
<script name='testScript1' id='#0000'>

    <!-- page1 -->
    <page name='page1' id='#0001'>
        <print text='hello'/>
        <goto-page name='page2'/>
    </page>

    <!-- page2 -->
    <page name='page2' id='#0002'>
        <print text='world'/>
    </page>
</script>
";
            var idsString = @"
#0000: b110a4ed-a36b-439e-908f-6239accf855c
#0001: 92bec51d-fcea-4815-bd08-cd7fbc620fcc
#0002: 685cf2fd-988c-49eb-a411-5492d2e0ece2
";

            var ids = new ScriptIds();
            ids.Parse(idsString);

            var xParser = new XParser();
            var result = xParser.Process(ids, xml);

            var root = result.Children[0];

            Assert.NotNull(result);
            Assert.AreEqual("testScript1", root.Name);
            Assert.AreEqual("b110a4ed-a36b-439e-908f-6239accf855c", root.Id.ToString());
            Assert.AreEqual(2, root.Children.Count);

            var page1 = root.Children[0];
            var page2 = root.Children[1];

            Assert.AreEqual("92bec51d-fcea-4815-bd08-cd7fbc620fcc", page1.Id.ToString());
            Assert.AreEqual("685cf2fd-988c-49eb-a411-5492d2e0ece2", page2.Id.ToString());
        }

        [Test]
        public void DruidsTest()
        {

            var scriptIds = new ScriptIds();
            var ids = File.ReadAllText(TestHelper.directory + "/TestScripts/nodes/dialog_druids_sample_noids.ids");

            scriptIds.Parse(ids);


            var xParser = new XParser();
            var xml = File.ReadAllText(TestHelper.directory + "/TestScripts/nodes/dialog_druids_sample_noids.xml");

            var sw1 = Stopwatch.StartNew();

            var result = xParser.Process(scriptIds, xml);

            var root = result.Children[0];

            var bParser = new BinarySerialiser();

            var stringTable = new StringTable();
            var bytes = bParser.SerializeXTree(root, ref stringTable);

            sw1.Stop();

            Console.WriteLine("Time taken parse XML from memory: " + sw1.ElapsedMilliseconds + "ms");
            //File.WriteAllBytes("c:/temp/bytes.bytes", bytes);

            {
                var sw = Stopwatch.StartNew();
                var strings = stringTable.Serialise();
                //File.WriteAllBytes("c:/temp/bytes.strings", strings);

                stringTable.Deserialise(strings);
                var scriptFile = new CompiledScript();
                scriptFile.Deserialise(bytes);
                sw.Stop();

                Console.WriteLine("Time taken parse Binary from memory: " + sw.ElapsedMilliseconds + "ms");
            }
        }

        [Test]
        public void DruidsTestRepeat()
        {
            DruidsTest();
        }
        }
}
