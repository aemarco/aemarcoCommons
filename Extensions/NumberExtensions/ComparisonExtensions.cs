namespace aemarcoCommons.Extensions.NumberExtensions;

public static class ComparisonExtensions
{

    public static bool IsNearlyEqual(this double number, double compareValue, double epsilon = 0.0000001d)
    {
        const double minNormal = 2.2250738585072014E-308d;
        var absA = Math.Abs(number);
        var absB = Math.Abs(compareValue);
        var diff = Math.Abs(number - compareValue);

        if (number.Equals(compareValue))
        { // shortcut, handles infinities
            return true;
        }
        else if (number.Equals(0) || compareValue.Equals(0) || absA + absB < minNormal)
        {
            // a or b is zero or both are extremely close to it
            // relative error is less meaningful here
            return diff < (epsilon * minNormal);
        }
        else
        { // use relative error
            return diff / (absA + absB) < epsilon;
        }
    }


}