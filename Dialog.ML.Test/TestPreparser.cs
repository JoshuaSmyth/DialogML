using DialogML;
using DialogML.VM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dialog.ML.Test
{
    [TestFixture]
    public class TestPreparser
    {
        [Test]
        public void Pass()
        {
            Assert.Pass();
        }

        [Test]
        public void PreparserTest_001()
        {
            var inputFile = TestHelper.directory + "/TestScripts/preparser/input_ids_001.xml";
            var preparser = new Preparser();

            preparser.Preparse(inputFile);

            var idsFile = TestHelper.directory + "/TestScripts/preparser/input_ids_001.ids";
            var scriptIds = new ScriptIds();
            var idsFileContents = File.ReadAllText(idsFile);
            scriptIds.Parse(idsFileContents);
        }

        [Test]
        public void PreparserTest_002()
        {
            var inputFile = TestHelper.directory + "/TestScripts/preparser/input_ids_002.xml";
            var preparser = new Preparser();

            preparser.Preparse(inputFile);

            var idsFile = TestHelper.directory + "/TestScripts/preparser/input_ids_002.ids";
            var scriptIds = new ScriptIds();
            var idsFileContents = File.ReadAllText(idsFile);
            scriptIds.Parse(idsFileContents);
        }
    }
}
