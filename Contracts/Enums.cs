using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contracts
{

    /// <summary>
    /// Severity of a log message 0 Trace, 1 Debug, 2 Info, 3 Warn, 4 Error, 5 Fatal
    /// </summary>
    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
       
    }


}
