using System;
using System.Drawing;
using Extensions.PictureExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace ExtensionsTests.PictureExtensionsTests
{
    public class RatioExtensionsTests
    {
        [TestCase(1920,1080,20,1.42)] //picture 1920 * 1350
        [TestCase(1920, 1080, 10, 1.6)] //picture 1920 * 1200
        [TestCase(500, 900, 10, 0.5)] //picture 500 * 1000
        [TestCase(500, 800, 20, 0.5)] // picture 500 * 1000
        [TestCase(1000, 1000, 0, 1)] // no cut, picture 1000 * 1000
        [TestCase(1000, int.MaxValue, 99, 0)] //handles huge number
        [TestCase(1000, int.MaxValue, 100, 0)] //handles infinity
        [TestCase(1000, 1000, 100, 0)] //handles infinity
        public void ToMinRatio_DeliversCorrectly(int targetWidth, int targetHeight, int cutAllowed, double expected)
        {
            var rect = new Size(targetWidth, targetHeight);
            var result = rect.ToMinRatio(cutAllowed);

            result.Should().BeInRange(expected - 0.01, expected + 0.01);
        }


        [TestCase(1000, 1000, 101)]
        [TestCase(1000, 1000, -1)]
        [TestCase(0, 10, 50)]
        [TestCase(10, 0, 50)]
        public void ToMinRatio_ThrowsArgumentOutOfRangeException(int targetWidth, int targetHeight, int cutAllowed)
        {
            var rect = new Size(targetWidth, targetHeight);

            rect.Invoking(x => x.ToMinRatio(cutAllowed))
                .Should().Throw<ArgumentOutOfRangeException>();
        }



        [TestCase(1920, 1080, 20, 2.22)] //picture 2400 * 1080
        [TestCase(1920, 1080, 10, 1.98)] //picture 2133 * 1080
        [TestCase(500, 1000, 10, 0.56)] //picture 555 * 1000
        [TestCase(500, 1000, 20, 0.62)] // picture 625 * 1000
        [TestCase(1000, 1000, 0, 1)] // no cut, picture 1000 * 1000
        [TestCase(int.MaxValue, 1000, 100, 1.7976931348623157E+308)] //handles infinity
        [TestCase(1000, 1000, 100, 1.7976931348623157E+308)] //handles infinity
        public void ToMaxRatio_DeliversCorrectly(int targetWidth, int targetHeight, int cutAllowed, double expected)
        {
            var rect = new Size(targetWidth, targetHeight);
            var result = rect.ToMaxRatio(cutAllowed);

            result.Should().BeInRange(expected - 0.01, expected + 0.01);
        }


        [TestCase(1000, 1000, 101)]
        [TestCase(1000, 1000, -1)]
        [TestCase(0, 10, 50)]
        [TestCase(10, 0, 50)]
        public void ToMaxRatio_ThrowsArgumentOutOfRangeException(int targetWidth, int targetHeight, int cutAllowed)
        {
            var rect = new Size(targetWidth, targetHeight);

            rect.Invoking(x => x.ToMaxRatio(cutAllowed))
                .Should().Throw<ArgumentOutOfRangeException>();
        }

    }
}
