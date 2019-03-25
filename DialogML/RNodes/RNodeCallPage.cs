using DialogML.DNodes;
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

     //   public string PageName;

        public Guid PageId;

        public RNodeCallPage(Guid id, Guid pageId)
        {
            Id = id;
          //  PageName = pageName;
            PageId = pageId;
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

                // FIXEME Pagepage cannot lookup by id yet?
               // executionUnit.CallPageRegister = PageName;
                executionUnit.CallPageId = PageId;
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
