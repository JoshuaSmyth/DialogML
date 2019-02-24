using DialogML.DNodes;
using System;

namespace DialogML.RNodes
{
    public class RNodeCaseFalse : RNode
    {
        public Guid Id;
        public RNodeCaseFalse(Guid id)
        {
            Id = id;
        }

        public override AdvanceType Execute(ScriptApi api)
        {
            // TODO Eval
            api.Trace("RNodeCaseFalse");

            return AdvanceType.FirstChild;
        }
    }
}
