

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Extensions;

public static class RangeExtensions
{

    // Clamp
    public static int Clamp(this int value, int inclusiveMinimum, int inclusiveMaximum)
    {
        if (inclusiveMinimum > inclusiveMaximum)
            throw new ArgumentException("Minimum cannot be greater than maximum.");
        return Math.Max(inclusiveMinimum, Math.Min(inclusiveMaximum, value));
    }
    public static float Clamp(this float value, float inclusiveMinimum, float inclusiveMaximum)
    {
        if (inclusiveMinimum > inclusiveMaximum)
            throw new ArgumentException("Minimum cannot be greater than maximum.");
        return Math.Max(inclusiveMinimum, Math.Min(inclusiveMaximum, value));
    }
    public static double Clamp(this double value, double inclusiveMinimum, double inclusiveMaximum)
    {
        if (inclusiveMinimum > inclusiveMaximum)
            throw new ArgumentException("Minimum cannot be greater than maximum.");
        return Math.Max(inclusiveMinimum, Math.Min(inclusiveMaximum, value));
    }

    // NormalizeExtrapolate
    public static float NormalizeExtrapolate(this float value,
        float min, float max,
        float targetMin = 0, float targetMax = 1)
    {
        if (max - min == 0)
            throw new ArgumentException("Max and min cannot be equal.");
        var result = (value - min) / (max - min) * (targetMax - targetMin) + targetMin;
        return result;
    }
    public static double NormalizeExtrapolate(this double value,
        double min, double max,
        double targetMin = 0, double targetMax = 1)
    {
        if (max - min == 0)
            throw new ArgumentException("Max and min cannot be equal.");
        var result = (value - min) / (max - min) * (targetMax - targetMin) + targetMin;
        return result;
    }

    // NormalizeToRange
    public static float NormalizeToRange(this float value,
        float min, float max,
        float targetMin = 0, float targetMax = 1)
    {
        var result = value.NormalizeExtrapolate(min, max, targetMin, targetMax);
        return result.Clamp(targetMin, targetMax);
    }
    public static double NormalizeToRange(this double value,
        double min, double max,
        double targetMin = 0, double targetMax = 1)
    {
        var result = value.NormalizeExtrapolate(min, max, targetMin, targetMax);
        return result.Clamp(targetMin, targetMax);
    }

    // RangeMidpoint
    public static float RangeMidpoint(float min, float max)
    {
        if (min > max)
            throw new ArgumentException("Minimum cannot be greater than maximum.");
        return (min + max) / 2f;
    }
    public static double RangeMidpoint(double min, double max)
    {
        if (min > max)
            throw new ArgumentException("Minimum cannot be greater than maximum.");
        return (min + max) / 2.0;
    }

    // IsInRange
    public static bool IsInRange(this int value, int inclusiveMinimum, int inclusiveMaximum)
    {
        if (inclusiveMinimum > inclusiveMaximum)
            throw new ArgumentException("Minimum cannot be greater than maximum.");
        return value >= inclusiveMinimum && value <= inclusiveMaximum;
    }
    public static bool IsInRange(this float value, float inclusiveMinimum, float inclusiveMaximum)
    {
        if (inclusiveMinimum > inclusiveMaximum)
            throw new ArgumentException("Minimum cannot be greater than maximum.");
        return value >= inclusiveMinimum && value <= inclusiveMaximum;
    }
    public static bool IsInRange(this double value, double inclusiveMinimum, double inclusiveMaximum)
    {
        if (inclusiveMinimum > inclusiveMaximum)
            throw new ArgumentException("Minimum cannot be greater than maximum.");
        return value >= inclusiveMinimum && value <= inclusiveMaximum;
    }

    // OverlapsWith
    public static bool OverlapsWith(int min1, int max1, int min2, int max2)
    {
        if (min1 > max1 || min2 > max2)
            throw new ArgumentException("Minimum cannot be greater than maximum.");
        return min1 <= max2 && min2 <= max1;
    }
    public static bool OverlapsWith(float min1, float max1, float min2, float max2)
    {
        if (min1 > max1 || min2 > max2)
            throw new ArgumentException("Minimum cannot be greater than maximum.");
        return min1 <= max2 && min2 <= max1;
    }
    public static bool OverlapsWith(double min1, double max1, double min2, double max2)
    {
        if (min1 > max1 || min2 > max2)
            throw new ArgumentException("Minimum cannot be greater than maximum.");
        return min1 <= max2 && min2 <= max1;
    }
}