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
        /// Exception based on parameter name
        /// </summary>
        /// <param name="parameterName">name of the parameter</param>
        public BadRequestException(string parameterName)
            : base($"Reason ArgumentException '{parameterName}'")
        {
            
        }
    }
}
