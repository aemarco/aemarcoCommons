using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions.netExtensions
{
    public static class ExceptionExtensions
    {

        public static Exception Unpack(this Exception exception)
        {
            var result = exception;
            while (result?.InnerException != null)
            {
                result = result.InnerException;
            }
            return result;
        }




    }
}
