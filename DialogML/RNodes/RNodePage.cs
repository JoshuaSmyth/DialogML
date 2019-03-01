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
        //NodePageState state;

        public Guid Id;
        public String Name;

        public RNodePage(Guid id, String name)
        {
            Id = id;
            Name = name;
        }

        public override AdvanceType Execute(ScriptApi api)
        {
            api.PushReturnParentNode();
            return AdvanceType.FirstChild;
            /*
            if(state == NodePageState.Preevaluated)
            {
                state = NodePageState.Evaluated;
                api.Trace("PageNode");
                api.PushReturnParentNode();
                return AdvanceType.FirstChild;
            }
            else
            {
                api.Trace("PageNode:Return");
                return AdvanceType.Next;
            }*/
        }
    }
}
