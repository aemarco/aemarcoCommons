using System;
using System.Linq;

namespace aemarcoCommons.Extensions.FileExtensions;

public static class StorageSizeStuff
{

    public static decimal ConvertFromTo(this int value, string sourceUnit, string targetUnit)
    {
        return ((long)value).ConvertFromTo(sourceUnit, targetUnit);
    }

    public static decimal ConvertFromTo(this long value, string sourceUnit, string targetUnit)
    {
        var result = Convert.ToDecimal(value);
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };

        sourceUnit = sourceUnit.ToUpper();
        targetUnit = targetUnit.ToUpper();
        if (!sizes.Contains(sourceUnit)) throw new ArgumentOutOfRangeException(nameof(sourceUnit));
        if (!sizes.Contains(targetUnit)) throw new ArgumentOutOfRangeException(nameof(targetUnit));


        //convert to byte
        for (var i = Array.IndexOf(sizes, sourceUnit); i > 0; i--)
        {
            result *= 1024;
        }

        //convert to target
        for (int i = 0; i < Array.IndexOf(sizes, targetUnit); i++)
        {
            result /= 1024;

        }
        return result;
    }


}