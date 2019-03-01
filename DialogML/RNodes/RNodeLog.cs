using DialogML.DNodes;
using System;

namespace DialogML.RNodes
{
    public enum LogFilter
    {
        Info = 0,
        Debug = 1,
        Warning = 2,
        Error = 3, 
        Critical = 4
    }
    
    public class RNodeLog: RNode
    {
        public LogFilter Filter;   // TODO Implement Custom Filters
        public String Text;
        public Guid Id;
        public RNodeLog(Guid id, LogFilter filter, string text)
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
