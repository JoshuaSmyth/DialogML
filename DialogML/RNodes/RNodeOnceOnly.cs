﻿using DialogML.DNodes;
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
        Guid Id;

        OnceOnlyConfig NodeConfig;

        public RNodeOnceOnly(Guid id, OnceOnlyConfig config)
        {
            Id = id;
            NodeConfig = config;
        }

        public override AdvanceType Execute(ScriptApi api)
        {
            api.Trace("Once only");
            if(State == NodeOnlyIfState.Preevaluated)
            {
                api.PushReturnCurrentNode();
                State = NodeOnlyIfState.PostEvaluated;
                if(api.HasOnlyIfBeenExecuted(Id))
                {

                    // Execute the true child
                    if(NodeConfig == OnceOnlyConfig.SingleChildTrue)
                    {
                        return AdvanceType.FirstChild;
                    }
                    if(NodeConfig == OnceOnlyConfig.TwinChildrenTrueFalse)
                    {
                        return AdvanceType.FirstChild;
                    }
                    if(NodeConfig == OnceOnlyConfig.TwinChildrenFalseTrue)
                    {
                        return AdvanceType.SecondChild;
                    }
                }
                else
                {

                    // Execute the false child
                    if(NodeConfig == OnceOnlyConfig.SingleChildFalse)
                    {
                        return AdvanceType.FirstChild;
                    }
                    if(NodeConfig == OnceOnlyConfig.TwinChildrenFalseTrue)
                    {
                        return AdvanceType.FirstChild;
                    }
                    if(NodeConfig == OnceOnlyConfig.TwinChildrenTrueFalse)
                    {
                        return AdvanceType.SecondChild;
                    }

                }
            }

            return AdvanceType.Next;
        }
    }
}
