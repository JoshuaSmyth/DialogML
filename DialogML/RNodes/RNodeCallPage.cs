using DialogML.DNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.RNodes
{
    public enum NodeCallState
    {
        precall = 0,
        postcall = 1
    }

    class RNodeCallPage : RNode
    {
        private NodeCallState State;

        public override AdvanceType Execute(ScriptApi api)
        {
            if (State == NodeCallState.postcall)
            {
                return AdvanceType.Next;
            }
            else
            {

                api.PushJumpToNode(); // Pass Guid and type?
                api.Trace("RNode Call Page");

                State = NodeCallState.postcall;
                return AdvanceType.JumpToNode;
            }
        }

        public override void Prep()
        {
            // NOOP
        }
    }
}
