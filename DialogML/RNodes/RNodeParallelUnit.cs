using DialogML.DNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.RNodes
{
    public class RNodeParallelUnit : RNode
    {
        public override AdvanceType Execute(ScriptApi api, ExecutionUnit executionUnit)
        {
            // TODO Implement Properly
            api.Trace("ParallelUnit");
            return AdvanceType.FirstChild;
        }

        public override void Prep()
        {
            // NOOP
        }
    }
}
