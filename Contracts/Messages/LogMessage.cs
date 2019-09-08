using System;

namespace Contracts.Messages
{
    public class LogMessage
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Environment { get; set; }
        public string App { get; set; }
        public string Source { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }

    }




}
