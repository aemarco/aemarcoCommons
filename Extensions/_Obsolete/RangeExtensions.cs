using System;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Extensions.NumberExtensions
{
    [Obsolete("This class is obsolete. Use Clamp instead.")]
    public static class RangeExtensions
    {
        // LimitToRange
        public static int LimitToRange(this int value, int inclusiveMinimum, int inclusiveMaximum) =>
            value.Clamp(inclusiveMinimum, inclusiveMaximum);
        public static float LimitToRange(this float value, float inclusiveMinimum, float inclusiveMaximum) =>
            value.Clamp(inclusiveMinimum, inclusiveMaximum);
        public static double LimitToRange(this double value, double inclusiveMinimum, double inclusiveMaximum) =>
            value.Clamp(inclusiveMinimum, inclusiveMaximum);
    }
}
