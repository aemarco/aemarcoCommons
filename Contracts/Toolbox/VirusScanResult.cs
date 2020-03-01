using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Toolbox
{
    public class VirusScanResult
    {

        private VirusScanResult(string filePath, bool success)
        {
            FilePath = filePath;
            Success = success;
        }

        public VirusScanResult(string filePath, bool isSafe, string message)
            :this(filePath, true)
        {
            IsSafe = isSafe;
            Message = message;
        }


        public VirusScanResult(string filePath, Exception exception)
            :this(filePath, false)
        {
            Exception = exception;
        }



        public string FilePath { get; }
        public bool Success { get; }
        public bool IsSafe { get; }
        public string Message { get; }
        public Exception Exception { get; }
    }





}
