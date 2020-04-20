using System;
using System.Net;

namespace Toolbox.ApiTools
{
    public class UnwantedResultArgs : EventArgs
    {
        public UnwantedResultArgs(string message, Exception ex, HttpStatusCode? status)
        {
            Message = message;
            Ex = ex;
            Status = status;
        }

        public string Message { get; }
        public Exception Ex { get; }
        public HttpStatusCode? Status { get; }
    }
}