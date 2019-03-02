using DialogML.DNodes;
using DialogML.XNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.RNodes
{
    public class RNodeOnceOnly : RNode
    {
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
            return AdvanceType.Next;
        }
    }
}
