using aemarcoCommons.Extensions;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace ExtensionsTests.NumberExtensionsTests;

public class RangeExtensionsTests
{

    // --- Clamp ---

    [TestCase(5, 0, 10, 5)]
    [TestCase(-5, 0, 10, 0)]
    [TestCase(15, 0, 10, 10)]
    [TestCase(int.MinValue, 0, 10, 0)]
    [TestCase(int.MaxValue, 0, 10, 10)]
    public void Clamp_Int_Works(int value, int min, int max, int expected)
    {
        value.Clamp(min, max).Should().Be(expected);
    }

    [TestCase(5.5f, 0f, 10f, 5.5f)]
    [TestCase(-5f, 0f, 10f, 0f)]
    [TestCase(15f, 0f, 10f, 10f)]
    [TestCase(float.MinValue, 0f, 10f, 0f)]
    [TestCase(float.MaxValue, 0f, 10f, 10f)]
    [TestCase(float.NegativeInfinity, 0f, 10f, 0f)]
    [TestCase(float.PositiveInfinity, 0f, 10f, 10f)]
    public void Clamp_Float_Works(float value, float min, float max, float expected)
    {
        value.Clamp(min, max).Should().BeApproximately(expected, 0.0001f);
    }

    [TestCase(5.5, 0, 10, 5.5)]
    [TestCase(-5, 0, 10, 0)]
    [TestCase(15, 0, 10, 10)]
    [TestCase(double.MinValue, 0, 10, 0)]
    [TestCase(double.MaxValue, 0, 10, 10)]
    [TestCase(double.NegativeInfinity, 0, 10, 0)]
    [TestCase(double.PositiveInfinity, 0, 10, 10)]
    public void Clamp_Double_Works(double value, double min, double max, double expected)
    {
        value.Clamp(min, max).Should().BeApproximately(expected, 0.0001);
    }

    [Test]
    public void Clamp_Throws_On_Invalid_Range()
    {
        Action actInt = () => 5.Clamp(10, 0);
        Action actFloat = () => 5f.Clamp(10f, 0f);
        Action actDouble = () => 5d.Clamp(10d, 0d);
        actInt.Should().Throw<ArgumentException>();
        actFloat.Should().Throw<ArgumentException>();
        actDouble.Should().Throw<ArgumentException>();
    }

    // --- NormalizeExtrapolate ---

    [TestCase(5, 0, 10, 0, 1, 0.5f)]
    [TestCase(-5, 0, 10, 0, 1, -0.5f)]
    [TestCase(15, 0, 10, 0, 1, 1.5f)]
    [TestCase(5, 0, 10, 1, 0, 0.5f)] // reverse normalization
    public void NormalizeExtrapolate_Float_Works(float value, float min, float max, float targetMin, float targetMax, float expected)
    {
        value.NormalizeExtrapolate(min, max, targetMin, targetMax).Should().BeApproximately(expected, 0.0001f);
    }

    [TestCase(5, 0, 10, 0, 1, 0.5)]
    [TestCase(-5, 0, 10, 0, 1, -0.5)]
    [TestCase(15, 0, 10, 0, 1, 1.5)]
    [TestCase(5, 0, 10, 1, 0, 0.5)] // reverse normalization
    public void NormalizeExtrapolate_Double_Works(double value, double min, double max, double targetMin, double targetMax, double expected)
    {
        value.NormalizeExtrapolate(min, max, targetMin, targetMax).Should().BeApproximately(expected, 0.0001);
    }

    [Test]
    public void NormalizeExtrapolate_Throws_On_ZeroRange()
    {
        Action actFloat = () => 5f.NormalizeExtrapolate(1f, 1f);
        Action actDouble = () => 5d.NormalizeExtrapolate(1d, 1d);
        actFloat.Should().Throw<ArgumentException>();
        actDouble.Should().Throw<ArgumentException>();
    }

    // --- NormalizeToRange ---

    [TestCase(5, 0, 10, 0, 1, 0.5f)]
    [TestCase(-5, 0, 10, 0, 1, 0)]
    [TestCase(15, 0, 10, 0, 1, 1)]
    public void NormalizeToRange_Float_Works(float value, float min, float max, float targetMin, float targetMax, float expected)
    {
        value.NormalizeToRange(min, max, targetMin, targetMax).Should().BeApproximately(expected, 0.0001f);
    }

    [TestCase(5, 0, 10, 0, 1, 0.5)]
    [TestCase(-5, 0, 10, 0, 1, 0)]
    [TestCase(15, 0, 10, 0, 1, 1)]
    public void NormalizeToRange_Double_Works(double value, double min, double max, double targetMin, double targetMax, double expected)
    {
        value.NormalizeToRange(min, max, targetMin, targetMax).Should().BeApproximately(expected, 0.0001);
    }

    [Test]
    public void NormalizeToRange_Throws_On_Invalid_TargetRange()
    {
        Action actFloat = () => 5f.NormalizeToRange(0, 10, 1, 0);
        Action actDouble = () => 5d.NormalizeToRange(0, 10, 1, 0);
        actFloat.Should().Throw<ArgumentException>();
        actDouble.Should().Throw<ArgumentException>();
    }





    // --- RangeMidpoint ---

    [TestCase(0, 10, 5)]
    [TestCase(-10, 10, 0)]
    [TestCase(-10, -5, -7.5f)]
    [TestCase(5, 5, 5)]
    public void RangeMidpoint_Float_Works(float min, float max, float expected)
    {
        RangeExtensions.RangeMidpoint(min, max).Should().BeApproximately(expected, 0.0001f);
    }

    [TestCase(0.0, 10.0, 5.0)]
    [TestCase(-10.0, 10.0, 0.0)]
    [TestCase(-10.0, -5.0, -7.5)]
    [TestCase(5.0, 5.0, 5.0)]
    public void RangeMidpoint_Double_Works(double min, double max, double expected)
    {
        RangeExtensions.RangeMidpoint(min, max).Should().BeApproximately(expected, 0.0001);
    }

    [Test]
    public void RangeMidpoint_Throws_On_Invalid_Range()
    {
        Action actFloat = () => RangeExtensions.RangeMidpoint(10f, 0f);
        Action actDouble = () => RangeExtensions.RangeMidpoint(10d, 0d);
        actFloat.Should().Throw<ArgumentException>();
        actDouble.Should().Throw<ArgumentException>();
    }

    // --- IsInRange ---

    [TestCase(0, 0, 10, true)]
    [TestCase(5, 0, 10, true)]
    [TestCase(10, 0, 10, true)]
    [TestCase(-1, 0, 10, false)]
    [TestCase(11, 0, 10, false)]
    public void IsInRange_Int_Works(int value, int min, int max, bool expected)
    {
        value.IsInRange(min, max).Should().Be(expected);
    }


    [TestCase(0, 0, 10, true)]
    [TestCase(5, 0, 10, true)]
    [TestCase(10, 0, 10, true)]
    [TestCase(-1, 0, 10, false)]
    [TestCase(11, 0, 10, false)]
    public void IsInRange_Float_Works(float value, float min, float max, bool expected)
    {
        value.IsInRange(min, max).Should().Be(expected);
    }

    [TestCase(0, 0, 10, true)]
    [TestCase(5, 0, 10, true)]
    [TestCase(10, 0, 10, true)]
    [TestCase(-0.1, 0, 10, false)]
    [TestCase(10.1, 0, 10, false)]
    public void IsInRange_Double_Works(double value, double min, double max, bool expected)
    {
        value.IsInRange(min, max).Should().Be(expected);
    }

    [Test]
    public void IsInRange_Throws_On_Invalid_Range()
    {
        Action actInt = () => 5.IsInRange(10, 0);
        Action actFloat = () => 5f.IsInRange(10f, 0f);
        Action actDouble = () => 5d.IsInRange(10d, 0d);
        actInt.Should().Throw<ArgumentException>();
        actFloat.Should().Throw<ArgumentException>();
        actDouble.Should().Throw<ArgumentException>();
    }

    // --- OverlapsWith ---

    [TestCase(0, 10, 5, 15, true)]
    [TestCase(0, 10, 11, 20, false)]
    [TestCase(0, 10, -5, 0, true)]
    [TestCase(0, 10, 0, 10, true)]
    [TestCase(5, 5, 5, 5, true)]
    public void OverlapsWith_Int_Works(int min1, int max1, int min2, int max2, bool expected)
    {
        RangeExtensions.OverlapsWith(min1, max1, min2, max2).Should().Be(expected);
    }

    [TestCase(0, 10, 5, 15, true)]
    [TestCase(0, 10, 11, 20, false)]
    [TestCase(0, 10, -5, 0, true)]
    [TestCase(0, 10, 0, 10, true)]
    [TestCase(5, 5, 5, 5, true)]
    public void OverlapsWith_Float_Works(float min1, float max1, float min2, float max2, bool expected)
    {
        RangeExtensions.OverlapsWith(min1, max1, min2, max2).Should().Be(expected);
    }

    [TestCase(0, 10, 5, 15, true)]
    [TestCase(0, 10, 11, 20, false)]
    [TestCase(0, 10, -5, 0, true)]
    [TestCase(0, 10, 0, 10, true)]
    [TestCase(5, 5, 5, 5, true)]
    public void OverlapsWith_Double_Works(double min1, double max1, double min2, double max2, bool expected)
    {
        RangeExtensions.OverlapsWith(min1, max1, min2, max2).Should().Be(expected);
    }

    [Test]
    public void OverlapsWith_Throws_On_Invalid_Range()
    {
        Action actInt = () => RangeExtensions.OverlapsWith(10, 0, 0, 10);
        Action actFloat = () => RangeExtensions.OverlapsWith(10f, 0f, 0f, 10f);
        Action actDouble = () => RangeExtensions.OverlapsWith(10d, 0d, 0d, 10d);
        actInt.Should().Throw<ArgumentException>();
        actFloat.Should().Throw<ArgumentException>();
        actDouble.Should().Throw<ArgumentException>();
    }
}