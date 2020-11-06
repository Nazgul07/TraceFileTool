using System;

namespace TraceFileTool
{
    public class Message
    {
        public long DebugId { get; set; }
        public string DisplayName { get; set; }

        public long Level { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsRequest { get; set; }
        public bool RequestFromManuScript { get; set; }
        public TimeSpan Total => End - Start;
    }
}
