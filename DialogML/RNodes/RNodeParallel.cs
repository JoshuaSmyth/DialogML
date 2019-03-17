using DialogML.DNodes;

namespace DialogML.RNodes
{
    public enum ParallelNodeState
    {
        EnqueNodes,
        AwaitResolution
    }

    public class RNodeParallel : RNode
    {
        ParallelNodeState State;

        public override AdvanceType Execute(ScriptApi api, ExecutionUnit executionUnit)
        {
            if(State == ParallelNodeState.EnqueNodes)
            {
                foreach(var c in Children)
                {
                    api.AddParallelUnit(c);
                }

                State = ParallelNodeState.AwaitResolution;
                return AdvanceType.Yield;
            }
            else
            {
                var c = api.CountParallelNodes(this);
                if(c == 0)
                {
                    return AdvanceType.Next;
                }
                else
                {
                    // If all parallel units are resolved then we are done.
                    //api.Trace("Parallel Yeild");
                    return AdvanceType.Yield;
                }
            }
        }

        public override void Prep()
        {
            // NOOP
            State = ParallelNodeState.EnqueNodes;
        }
    }
}
