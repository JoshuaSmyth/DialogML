using DialogML.DNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.RNodes
{
    public class RNodeParallel : RNode
    {
        public override AdvanceType Execute(ScriptApi api)
        {
            // TODO Implement Properly
            api.Trace("Parallel");
            return AdvanceType.FirstChild;
        }

        public override void Prep()
        {
            // NOOP
        }
    }
}
