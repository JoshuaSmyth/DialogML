using DialogML.Expressions;
using System;

namespace DialogML.DNodes
{
    public class RNodeOption : RNode
    {
        public bool RemoveOnSelect;

        public CompiledExpression OnlyIf;

        public RNodeOption(Guid id, CompiledExpression expression, bool removeOnSelect)
        {
            Id = id;
            RemoveOnSelect = removeOnSelect;
            OnlyIf = expression;
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
