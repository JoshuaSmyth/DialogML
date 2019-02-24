using DialogML.DNodes;
using System;

namespace DialogML.RNodes
{
    public class RNodeLog: RNode
    {
        public String Filter;   // TODO Implement
        public String Text;
        public Guid Id;
        public RNodeLog(Guid id, string filter, string text)
        {
            Id = id;
            Text = text;
            Filter = filter;
        }

        public override AdvanceType Execute(ScriptApi api)
        {
            // TODO Eval
            api.Trace("Log: " + Filter + " : " + Text);

            return AdvanceType.Next;
        }
    }
}
