using System;

namespace aemarcoCommons.WebTools.Exceptions
{
    public class BadRequestException : Exception
    {
        /// <summary>
        /// Exception based on Argument exception param name
        /// </summary>
        /// <param name="exception">argument exception with param name</param>
        public BadRequestException(ArgumentException exception) 
            : base($"Reason ArgumentException '{exception.ParamName}'", exception)
        {
            
        }

        /// <summary>
        /// Exception based on general message, will be prefixed with Reason and put in ''
        /// </summary>
        /// <param name="message">parameter name or message</param>
        public BadRequestException(string message)
            : base($"Reason '{message}'")
        {
            
        }
        
    }
}
