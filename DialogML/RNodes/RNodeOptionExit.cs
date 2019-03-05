using System;

namespace DialogML.DNodes
{
    public class RNodeOptionExit : RNode
    {
        //public Guid Id;
        public RNodeOptionExit(Guid id)
        {
            Id = id;
        }

        public override AdvanceType Execute(ScriptApi api)
        {
            return AdvanceType.FirstChild;
        }

        public override void Prep()
        {
            //throw new NotImplementedException();
        }
    }
}
