using aemarcoCommons.Extensions.TextExtensions;
using FluentAssertions;
using NUnit.Framework;
using System.Globalization;
using System.Threading;

namespace ExtensionsTests.TextExtensionsTests
{
    public class ParsingTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        //integers

        [TestCase("42", true, 42)]
        [TestCase("-42", true, -42)]
        [TestCase("42.5", false, null)]
        [TestCase("Bob", false, null)]
        public void TryParse_sByteCorrectly(string text, bool canParse, sbyte? expected)
        {
            var couldParse = text.TryParse<sbyte>(out var number);
            couldParse.Should().Be(canParse);
            number.HasValue.Should().Be(canParse);
            number.Should().Be(expected);
        }

        [TestCase("42", true, (byte)42)]
        [TestCase("-42", false, null)]
        [TestCase("42.5", false, null)]
        [TestCase("Bob", false, null)]
        public void TryParse_ByteCorrectly(string text, bool canParse, byte? expected)
        {
            var couldParse = text.TryParse<byte>(out var number);
            couldParse.Should().Be(canParse);
            number.HasValue.Should().Be(canParse);
            number.Should().Be(expected);
        }

        [TestCase("42", true, 42)]
        [TestCase("-42", true, -42)]
        [TestCase("42.5", false, null)]
        [TestCase("Bob", false, null)]
        public void TryParse_ShortCorrectly(string text, bool canParse, short? expected)
        {
            var couldParse = text.TryParse<short>(out var number);
            couldParse.Should().Be(canParse);
            number.HasValue.Should().Be(canParse);
            number.Should().Be(expected);
        }

        [TestCase("42", true, (ushort)42)]
        [TestCase("-42", false, null)]
        [TestCase("42.5", false, null)]
        [TestCase("Bob", false, null)]
        public void TryParse_ShortCorrectly(string text, bool canParse, ushort? expected)
        {
            var couldParse = text.TryParse<ushort>(out var number);
            couldParse.Should().Be(canParse);
            number.HasValue.Should().Be(canParse);
            number.Should().Be(expected);
        }

        [TestCase("42", true, 42)]
        [TestCase("-42", true, -42)]
        [TestCase("42.5", false, null)]
        [TestCase("Bob", false, null)]
        public void TryParse_IntCorrectly(string text, bool canParse, int? expected)
        {
            var couldParse = text.TryParse<int>(out var number);
            couldParse.Should().Be(canParse);
            number.HasValue.Should().Be(canParse);
            number.Should().Be(expected);
        }

        [TestCase("42", true, 42u)]
        [TestCase("-42", false, null)]
        [TestCase("42.5", false, null)]
        [TestCase("Bob", false, null)]
        public void TryParse_UIntCorrectly(string text, bool canParse, uint? expected)
        {
            var couldParse = text.TryParse<uint>(out var number);
            couldParse.Should().Be(canParse);
            number.HasValue.Should().Be(canParse);
            number.Should().Be(expected);
        }

        [TestCase("42", true, 42)]
        [TestCase("-42", true, -42)]
        [TestCase("42.5", false, null)]
        [TestCase("Bob", false, null)]
        public void TryParse_LongCorrectly(string text, bool canParse, long? expected)
        {
            var couldParse = text.TryParse<long>(out var number);
            couldParse.Should().Be(canParse);
            number.HasValue.Should().Be(canParse);
            number.Should().Be(expected);
        }

        [TestCase("42", true, 42ul)]
        [TestCase("-42", false, null)]
        [TestCase("42.5", false, null)]
        [TestCase("Bob", false, null)]
        public void TryParse_ULongCorrectly(string text, bool canParse, ulong? expected)
        {
            var couldParse = text.TryParse<ulong>(out var number);
            couldParse.Should().Be(canParse);
            number.HasValue.Should().Be(canParse);
            number.Should().Be(expected);
        }


        //floating

        [TestCase("42", true, 42f)]
        [TestCase("-42", true, -42f)]
        [TestCase("42.5", true, 42.5f)]
        [TestCase("Bob", false, null)]
        public void TryParse_FloatCorrectly(string text, bool canParse, float? expected)
        {
            var couldParse = text.TryParse<float>(out var number);
            couldParse.Should().Be(canParse);
            number.HasValue.Should().Be(canParse);
            number.Should().Be(expected);
        }

        [TestCase("42", true, 42d)]
        [TestCase("-42", true, -42d)]
        [TestCase("42.5", true, 42.5d)]
        [TestCase("Bob", false, null)]
        public void TryParse_DoubleCorrectly(string text, bool canParse, double? expected)
        {
            var couldParse = text.TryParse<double>(out var number);
            couldParse.Should().Be(canParse);
            number.HasValue.Should().Be(canParse);
            number.Should().Be(expected);
        }

        [TestCase("42", true, 42)]
        [TestCase("-42", true, -42)]
        [TestCase("42.5", true, 42.5)]
        [TestCase("Bob", false, null)]
        public void TryParse_DecimalCorrectly(string text, bool canParse, decimal? expected)
        {
            var couldParse = text.TryParse<decimal>(out var number);
            couldParse.Should().Be(canParse);
            number.HasValue.Should().Be(canParse);
            number.Should().Be(expected);
        }


    }
}
