using DialogML.DNodes;
using System;

namespace DialogML.RNodes
{
    public class RNodeCaseFalse : RNode
    {
       // public Guid Id;
        public RNodeCaseFalse(Guid id)
        {
            Id = id;
        }

        public override AdvanceType Execute(ScriptApi api, ExecutionUnit executionUnit)
        {
            // TODO Eval
            api.Trace("RNodeCaseFalse");

            return AdvanceType.FirstChild;
        }

        public override void Prep()
        {
            //throw new NotImplementedException();
        }
    }
}
