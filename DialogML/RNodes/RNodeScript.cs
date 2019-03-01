using System;

namespace DialogML.DNodes
{
    public enum NodeScriptState
    {
        Preevaluated = 0,
        Evaluated = 1
    }

    public class RNodeScript : RNode
    {
        NodeScriptState State;

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
            if(State == NodeScriptState.Preevaluated)
            {
                State = NodeScriptState.Evaluated;
                return AdvanceType.FirstChild;
            }
            else
            {
                return AdvanceType.Next;
            }
        }
    }
}
