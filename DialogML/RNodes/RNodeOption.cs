using System;

namespace DialogML.DNodes
{
    public class RNodeOption : RNode
    {
        // TODO Only if expression
        public bool RemoveOnSelect;

        public RNodeOption(Guid id, bool removeOnSelect)
        {
            Id = id;
            RemoveOnSelect = removeOnSelect;
        }

        public override AdvanceType Execute(ScriptApi api, ExecutionUnit executionUnit)
        {
            return AdvanceType.FirstChild;
        }

        public override void Prep()
        {
            //throw new NotImplementedException();
        }
    }
}
