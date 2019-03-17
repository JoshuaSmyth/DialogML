using DialogML.DNodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.RNodes
{
    public enum WaitModeState
    {
        NotStarted = 0,
        Started = 1,
        Finished = 2
    }

    public class RNodeWait : RNode
    {
        WaitModeState state;    // TODO Shift state byte to base class and access via runtime table

        Int32 Milliseconds;
        Stopwatch sw = new Stopwatch(); // TODO Grab from some pool
        public RNodeWait(Guid id, Int32 milliseconds)
        {
            Id = id;
            Milliseconds = milliseconds;
            state = WaitModeState.NotStarted;
        }

        public override AdvanceType Execute(ScriptApi api, ExecutionUnit executionUnit)
        {
            if (state == WaitModeState.NotStarted)
            {
                sw.Start();
                state = WaitModeState.Started;
                return AdvanceType.Yield;
            }
            if (state == WaitModeState.Started)
            {
                if (sw.ElapsedMilliseconds >= Milliseconds)
                {
                    sw.Stop();
                    state = WaitModeState.Finished;
                    return AdvanceType.Next;
                }
                return AdvanceType.Yield;
            }

            //api.Trace("RNode: Wait");
            return AdvanceType.Next;
        }

        public override void Prep()
        {
            //throw new NotImplementedException();
        }
    }
}
