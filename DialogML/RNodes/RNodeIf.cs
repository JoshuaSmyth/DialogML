using DialogML.DNodes;
using System;

namespace DialogML.RNodes
{
    public enum NodeIfState
    {
        Preevaluated = 0,
        PostEvaluated = 11
    }

    public class RNodeIf : RNode
    {
        NodeIfState state;

        public String Expression;   // TODO Implement (Requires expression table)
        public Guid Id;
        public RNodeIf(Guid id)
        {
            Id = id;
        }

        public override AdvanceType Execute(ScriptApi api)
        {
            // TODO Eval and choose node

            api.Trace("NodeIf");

            if(state == NodeIfState.Preevaluated)
            {
                api.PushReturnCurrentNode();

                state = NodeIfState.PostEvaluated;

                return AdvanceType.FirstChild;
            }

            return AdvanceType.Next;
        }
    }
}
