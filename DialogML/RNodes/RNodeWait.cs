using DialogML.DNodes;
using System;
using System.Collections.Generic;
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
        Int32 Milliseconds;
        public RNodeWait(Guid id, Int32 milliseconds)
        {
            Id = id;
            Milliseconds = milliseconds;
        }

        public override AdvanceType Execute(ScriptApi api)
        {
            api.Trace("RNode: Wait");
            return AdvanceType.Next;
        }

        public override void Prep()
        {
            //throw new NotImplementedException();
        }
    }
}
