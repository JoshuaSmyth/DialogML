using DialogML.DNodes;
using DialogML.Expressions;
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

        public CompiledExpression Expression;
        public Guid Id;
        public RNodeIf(Guid id, CompiledExpression expression)
        {
            Id = id;
            Expression = expression;
        }

        public override AdvanceType Execute(ScriptApi api)
        {
            // TODO Evaluate Expression

            api.Trace("NodeIf");

            if(state == NodeIfState.Preevaluated)
            {
                var result = api.EvaluateExpression(Expression);
                api.PushReturnCurrentNode();
                state = NodeIfState.PostEvaluated;

                if(result == 1)
                {
                    // True
                    return AdvanceType.FirstChild;
                }
                else
                {
                    // False
                    return AdvanceType.SecondChild;
                }
            }

            return AdvanceType.Next;
        }
    }
}
