using System;

namespace DialogML.DNodes
{
    public class RNodePage : RNode
    {
        public Guid Id;
        public String Name;

        public RNodePage(Guid id, String name)
        {
            Id = id;
            Name = name;
        }

        public override AdvanceType Execute(ScriptApi api)
        {
            api.Trace("PageNode");

            return AdvanceType.FirstChild;
        }
    }
}
