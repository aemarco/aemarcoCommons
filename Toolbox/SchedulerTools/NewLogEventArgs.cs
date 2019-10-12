using System;

namespace Toolbox.SchedulerTools
{
    public class NewLogEventArgs : EventArgs
    {
        public NewLogEventArgs(string priority, string message, Exception ex)
        {
            Message = message;
            Priority = priority;
            Exception = ex;
        }

        public string Priority { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
