using System;
using System.Collections.Generic;
using System.Text;
using Extensions.netExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace ExtensionsTests.netExtensionTests
{
    public class DateTimeOffsetExtensionsTests
    {

        [TestCase(5, 6, true)]
        [TestCase(5, 4, false)]
        public void IsYoungerThan_ReturnsCorrect(int hoursInPast, int hoursInterval, bool expected)
        {
            var timestamp = DateTimeOffset.Now.AddHours(-hoursInPast);
            var timespan = TimeSpan.FromHours(hoursInterval);

            var result = timestamp.IsYoungerThan(timespan);

            result.Should().Be(expected);
        }


        [TestCase(5, 6, false)]
        [TestCase(5, 4, true)]
        public void IsOlderThan_ReturnsCorrect(int hoursInPast, int hoursInterval, bool expected)
        {
            var timestamp = DateTimeOffset.Now.AddHours(-hoursInPast);
            var timespan = TimeSpan.FromHours(hoursInterval);

            var result = timestamp.IsOlderThan(timespan);

            result.Should().Be(expected);
        }
    }
}
