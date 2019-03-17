using System;

namespace DialogML.DNodes
{
    // Note: These states need to be effemeral for the runtime of the
    // Script
    public enum NodePageState
    {
        Preevaluated = 0,
        Evaluated = 11
    }

    public class RNodePage : RNode
    {
        // State
        NodePageState state;

        //public Guid Id;
        public String Name;

        public RNodePage(Guid id, String name)
        {
            Id = id;
            Name = name;
        }

        public override AdvanceType Execute(ScriptApi api, ExecutionUnit executionUnit)
        {
           // api.PushReturnParentNode();
           // return AdvanceType.FirstChild;
            
            if(state == NodePageState.Preevaluated)
            {
                state = NodePageState.Evaluated;
                api.Trace("PageNode");
                // The parent might not be ready for re-entry
                // This might need to be more abstracted
                api.PushReturnCurrentNode();
                return AdvanceType.FirstChild;
            }
            else
            {
                api.Trace("PageNode:Return");

                // TODO Change the return type to goto parent
                return AdvanceType.Parent;
            }
        }

        public override void Prep()
        {
            state = NodePageState.Preevaluated;
            //throw new NotImplementedException();
        }
    }
}
