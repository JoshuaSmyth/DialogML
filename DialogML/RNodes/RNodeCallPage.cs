﻿using DialogML.DNodes;
using System;

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

        public RNodeCallPage(String pageName)
        {
            PageName = pageName;
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

                executionUnit.CallPageRegister = PageName;
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
