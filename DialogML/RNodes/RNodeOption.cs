using System;

namespace DialogML.DNodes
{
    public class RNodeOption : RNode
    {
        public Guid Id;
        public RNodeOption(Guid id)
        {
            Id = id;
        }

        public override AdvanceType Execute(ScriptApi api)
        {
            return AdvanceType.FirstChild;
        }
    }
}
