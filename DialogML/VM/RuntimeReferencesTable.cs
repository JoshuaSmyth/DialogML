using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DialogML.DNodes;
using DialogML.RNodes;

namespace DialogML.VM
{
    public class RuntimeReferencesTable
    {
        private Dictionary<Guid, RNodePage> Pages = new Dictionary<Guid, RNodePage>();
        
        internal void AddOrUpdateScript(CompiledScript script)
        {
            AddPagesRecursive(script.Root);
        }

        public RNodePage GetPageByName(String name)
        {
            // This is a slower method but just a test until the lookups are pre-compiled
            foreach(var p in Pages)
            {
                if (p.Value.Name == name)
                {
                    return p.Value;
                }
            }

            return null;
        }

        public RNodePage GetPageById(Guid id)
        {
            if (Pages.ContainsKey(id))
            {
                return Pages[id];
            }

            return null;
        }

        private void AddPagesRecursive(RNode root)
        {
            if (root is RNodePage)
            {
                var page = root as RNodePage;
                if (Pages.ContainsKey(page.Id))
                {

                }
                else
                {
                    Pages.Add(page.Id, page);
                }
            }
            foreach(var p in root.Children)
            {
                AddPagesRecursive(p);
            }
        }
    }
}
