using System;
using System.Collections.Generic;

namespace DialogML.DNodes
{
    public interface IRNode
    {
        Guid Id { get; set; }
    }

    public abstract class RNode : IRNode
    {
        public List<RNode> Children = new List<RNode>();

        public Guid Id { get; set; }

        public abstract void Prep();

        public abstract AdvanceType Execute(ScriptApi api);
    }
}
