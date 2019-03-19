using DialogML.DNodes;
using System;

namespace DialogML.RNodes
{
    class RNodeCallScript : RNode
    {
        private NodeCallState State;

        public string PageName;
        public string ScriptName;

        public RNodeCallScript(String scriptName, String pageName)
        {
            PageName = pageName;
            ScriptName = scriptName;
        }

        public override AdvanceType Execute(ScriptApi api, ExecutionUnit executionUnit)
        {
            if(State == NodeCallState.postcall)
            {
                return AdvanceType.Next;
            }
            else
            {
                api.Trace("RNode Call Page");

                State = NodeCallState.postcall;

                executionUnit.CallScriptRegister = ScriptName;
                executionUnit.CallPageRegister = PageName;
                return AdvanceType.JumpToNode;
            }
        }

        public override void Prep()
        {
            State = NodeCallState.precall;
        }
    }
}
