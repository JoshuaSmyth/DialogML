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
    }
}
