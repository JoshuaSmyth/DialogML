using DialogML.XNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.DNodes
{
    public class RNodeSay : RNode
    {
        //public Guid Id;
        public SayPosition SayPosition;
        public String ActorId;

        public RNodeSay(Guid id, SayPosition sayPosition, String actorId)
        {
            Id = id;
            SayPosition = sayPosition;
            ActorId = actorId;
        }

        public override AdvanceType Execute(ScriptApi api)
        {
            api.Say(ActorId, Id);
            return AdvanceType.Next;
        }

        public override void Prep()
        {
            //throw new NotImplementedException();
        }
    }
}
