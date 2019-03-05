using DialogML.DNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogML.RNodes
{
    public class RNodeRoot : RNode
    {
        public override AdvanceType Execute(ScriptApi api)
        {
            return AdvanceType.Next;
            //throw new NotImplementedException();
        }

        public override void Prep()
        {
            //throw new NotImplementedException();
        }
    }
}
