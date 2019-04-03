using DialogML.DNodes;
using System;

namespace DialogML.RNodes
{
    public class RNodeSet : RNode
    {
        //public Guid Id;
        public RNodeSet(Guid id)
        {
            Id = id;
        }

        public override AdvanceType Execute(ScriptApi api, ExecutionUnit executionUnit)
        {
            // TODO Eval
            api.Trace("TODO RNodeSet");

            return AdvanceType.Next;
        }

        public override void Prep()
        {
            // throw new NotImplementedException();
        }
    }
}
