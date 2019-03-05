using DialogML.DNodes;
using System;

namespace DialogML.RNodes
{
    public class RNodeCaseTrue : RNode
    {
        //public Guid Id;
        public RNodeCaseTrue(Guid id)
        {
            Id = id;
        }

        public override AdvanceType Execute(ScriptApi api)
        {
            // TODO Eval
            api.Trace("RNodeCaseTrue");

            return AdvanceType.FirstChild;
        }

        public override void Prep()
        {
           // throw new NotImplementedException();
        }
    }
}
