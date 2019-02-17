using System;

namespace DialogML.DNodes
{
    public class RNodeScript : RNode
    {
        public Guid Id;
        public String Name;

        public RNodeScript(Guid id, String name)
        {
            Id = id;
            Name = name;
        }

        public override AdvanceType Execute(ScriptApi api)
        {
            api.Trace("ScriptNode");

            return AdvanceType.FirstChild;
        }
    }
}
