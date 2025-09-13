using System;

namespace aemarcoCommons.Extensions.ExceptionExtensions;

public static class ReadingStuff
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