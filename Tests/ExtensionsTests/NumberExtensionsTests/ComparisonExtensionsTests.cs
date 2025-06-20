using aemarcoCommons.Extensions.NumberExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace ExtensionsTests.NumberExtensionsTests;

public class ComparisonExtensionsTests
{

    //[TestCase(5,5,0,true)]
    [TestCase(0.500001, 0.500002, 0.000001, true)]
    [TestCase(5.000001, 5.000002, 0.0000005, true)]
    [TestCase(5.000001, 5.000003, 0.0000001, false)]
    public void IsNearlyEqual_Rocks(double left, double right, double epsilon, bool expected)
    {
        var result = left.IsNearlyEqual(right, epsilon);
        result.Should().Be(expected);
    }




}