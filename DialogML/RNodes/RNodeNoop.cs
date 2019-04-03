using DialogML.DNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.RNodes
{
    class RNodeNoop : RNode
    {
        public override AdvanceType Execute(ScriptApi api, ExecutionUnit executionUnit)
        {
            return AdvanceType.Next;
        }

        public override void Prep()
        {
            //throw new NotImplementedException();
        }
    }
}
