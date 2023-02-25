using aemarcoCommons.Extensions.NumberExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace ExtensionsTests.NumberExtensionsTests;

public class RangeExtensionsTests
{


    [TestCase(5, 4, 6, 5)]
    [TestCase(5, 6, 10, 6)]
    [TestCase(5, 0, 4, 4)]
    public void LimitToRange_LimitsCorrectly(double val, double min, double max, double expected)
    {
        var result = val.LimitToRange(min, max);
        result.Should().Be(expected);
    }
}