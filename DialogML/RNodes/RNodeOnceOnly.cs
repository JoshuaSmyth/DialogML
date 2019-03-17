using DialogML.DNodes;
using DialogML.XNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.RNodes
{
    public enum NodeOnlyIfState
    {
        Preevaluated = 0,
        PostEvaluated = 11
    }

    public class RNodeOnceOnly : RNode
    {
        NodeOnlyIfState State;

        // TODO Id should be moved to the base
        //Guid Id;

        OnceOnlyConfig NodeConfig;

        public RNodeOnceOnly(Guid id, OnceOnlyConfig config)
        {
            Id = id;
            NodeConfig = config;
        }

        public override AdvanceType Execute(ScriptApi api, ExecutionUnit executionUnit)
        {
            api.Trace("Once only");
            if(State == NodeOnlyIfState.Preevaluated)
            {
                api.PushReturnCurrentNode();
                State = NodeOnlyIfState.PostEvaluated;
                if(api.HasOnceOnlyBeenExecuted(Id))
                {

                    // Execute the true child
                    if(NodeConfig == OnceOnlyConfig.SingleChildFalse)
                    {
                        return AdvanceType.FirstChild;
                    }
                    if(NodeConfig == OnceOnlyConfig.TwinChildrenTrueFalse)
                    {
                        return AdvanceType.SecondChild;
                    }
                    if(NodeConfig == OnceOnlyConfig.TwinChildrenFalseTrue)
                    {
                        return AdvanceType.FirstChild;
                    }
                }
                else
                {
                    api.MarkOnceOnlyAsVisited(Id);

                    // Execute the false child
                    if(NodeConfig == OnceOnlyConfig.SingleChildTrue)
                    {
                        return AdvanceType.FirstChild;
                    }
                    if(NodeConfig == OnceOnlyConfig.TwinChildrenFalseTrue)
                    {
                        return AdvanceType.SecondChild;
                    }
                    if(NodeConfig == OnceOnlyConfig.TwinChildrenTrueFalse)
                    {
                        return AdvanceType.FirstChild;
                    }

                }
            }

            return AdvanceType.Next;
        }

        public override void Prep()
        {
            State = NodeOnlyIfState.Preevaluated;
            //throw new NotImplementedException();
        }
    }
}
