using DialogML.DNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.RNodes
{
    public class RNodeReturn : RNode
    {
        public override AdvanceType Execute(ScriptApi api)
        {
            return AdvanceType.Finished;
        }

        public override void Prep()
        {
            // NOOP
        }
    }
}
