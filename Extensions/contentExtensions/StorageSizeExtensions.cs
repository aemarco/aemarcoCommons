using System;
using System.Linq;

namespace Extensions.contentExtensions
{
    public static class StorageSizeExtensions
    {

        public static long ConvertFromTo(this int value, string sourceUnit, string targetUnit)
        {
            return ((long)value).ConvertFromTo(sourceUnit, targetUnit);
        }

        public static long ConvertFromTo(this long value, string sourceUnit, string targetUnit)
        {
            long result = value;
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };

            sourceUnit = sourceUnit.ToUpper();
            if (!sizes.Contains(sourceUnit))
                throw new ArgumentOutOfRangeException(nameof(sourceUnit));
            for (int i = Array.IndexOf(sizes, sourceUnit); i > 0; i--)
            {
                result *= 1024;
            }


            targetUnit = targetUnit.ToUpper();
            for (int i = 0; i < sizes.Length; i++)
            {
                if (targetUnit == sizes[i])
                {
                    break;
                }
                if (i + 1 == sizes.Length)
                    throw new ArgumentOutOfRangeException(nameof(targetUnit));
                result /= 1024;
            }

            return result;
        }









    }
}
