﻿using System;

namespace DialogML.DNodes
{
    public class RNodeOption : RNode
    {
        // TODO Add OnlyIf expression
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
