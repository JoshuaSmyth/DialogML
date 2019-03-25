using DialogML.XNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML
{
    public class PageRecord
    {
        public Guid PageId;
        public Guid ScriptId;
        public String PageName;
    }

    public class ScriptRecord
    {
        public Guid ScriptId;
        public String ScriptName;
    }

    public class ReferencesTable
    {
        Dictionary<Guid, ScriptRecord> Scripts = new Dictionary<Guid, ScriptRecord>();
        Dictionary<Guid, Dictionary<Guid, PageRecord>> Pages = new Dictionary<Guid, Dictionary<Guid, PageRecord>>();

        public void AddScriptRecord(Guid id, String scriptName)
        {
            Scripts.Add(id, new ScriptRecord() { ScriptId = id, ScriptName = scriptName });
        }

        public void AddPageRecord(Guid scriptId, Guid pageId, String pageName)
        {
            if(!Pages.ContainsKey(scriptId))
            {
                Pages.Add(scriptId, new Dictionary<Guid, PageRecord>());
            }

            var dict = Pages[scriptId];
            dict.Add(pageId, new PageRecord() { PageId = pageId, PageName = pageName });
        }

        internal PageRecord GetPageRecord(string scriptName, string pageName)
        {
            foreach(var s in Scripts.Values)
            {
                if (s.ScriptName.ToLower() == scriptName.ToLower())
                {
                    return GetPageRecord(s.ScriptId, pageName);
                }
            }
            return null;
        }

        internal PageRecord GetPageRecord(Guid scriptId, string pageName)
        {
            if (!Pages.ContainsKey(scriptId))
            {
                return null;
            }
            var pages = Pages[scriptId];
            foreach(var p in pages.Values)
            {
                if (p.PageName.ToLower() == pageName.ToLower())
                {
                    return p;
                }
            }
            return null;
        }
    }

    public class PostParser
    {
        ReferencesTable m_ReferencesTable = new ReferencesTable();

        public void AddOrUpdateScript(XmlNode xmlNode, string filename)
        {
            AddOrUpdateScriptRecursive(Guid.Empty, filename, xmlNode);
        }

        private void AddOrUpdateScriptRecursive(Guid scriptId, string filename, XmlNode xmlNode)
        {
            if (xmlNode is XScript)
            {
                var script = xmlNode as XScript;
                scriptId = script.Id;
                m_ReferencesTable.AddScriptRecord(script.Id, filename);
            }
            if (xmlNode is XNodePage && scriptId != Guid.Empty)
            {
                var page = xmlNode as XNodePage;
                m_ReferencesTable.AddPageRecord(scriptId, page.Id, page.Name);
            }
            foreach(var c in xmlNode.Children)
            {
                AddOrUpdateScriptRecursive(scriptId, filename, c);
            }
        }

        public ReferencesTable GetReferencesTable()
        {
            return m_ReferencesTable;
        }
    }
}
