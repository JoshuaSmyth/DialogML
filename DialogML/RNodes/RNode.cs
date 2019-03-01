using System.Collections.Generic;

namespace DialogML.DNodes
{
    public class RNode
    {
        public List<RNode> Children = new List<RNode>();
        
        public virtual AdvanceType Execute(ScriptApi api)
        {
            api.Trace(typeof(RNode).ToString());

            return AdvanceType.Next;
        }

        public virtual void OnPrepareScript()
        {
            // The script is walked calling this method before running.
            // In order to clean up any old state that might be left behind.
        }
    }
}
