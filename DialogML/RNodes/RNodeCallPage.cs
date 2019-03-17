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

        public string PageName;

        public RNodeCallPage(String name)
        {
            PageName = name;
        }

        public override AdvanceType Execute(ScriptApi api, ExecutionUnit executionUnit)
        {
            if (State == NodeCallState.postcall)
            {
                return AdvanceType.Next;
            }
            else
            {
                api.Trace("RNode Call Page");

                State = NodeCallState.postcall;
                return AdvanceType.JumpToNode;
            }
        }

        public override void Prep()
        {
            State = NodeCallState.precall;
        }
    }
}
