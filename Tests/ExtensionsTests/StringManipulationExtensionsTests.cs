using aemarcoCommons.Extensions;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace ExtensionsTests;
internal class StringManipulationExtensionsTests
{

    [TestCase(null, "Abc", null)]
    [TestCase("PrefixText", null, "PrefixText")]
    [TestCase("", "Prefix", "")]
    [TestCase("", "", "")]
    [TestCase("PrefixText", "Prefix", "Text")]
    [TestCase("TextWithNoPrefix", "Prefix", "TextWithNoPrefix")]
    [TestCase("Prefix", "Prefix", "")]
    [TestCase("Prefix", "", "Prefix")]
    public void TrimStart_Works_WithOrdinal(string? text, string? remove, string? expected)
    {
        var result = text.TrimStart(remove);
        result.Should().Be(expected);
    }


    [TestCase(null, "Abc", null)]
    [TestCase("PrefixText", null, "PrefixText")]
    [TestCase("", "prefix", "")]
    [TestCase("", "", "")]
    [TestCase("PrefixText", "prefix", "Text")]
    [TestCase("TextWithNoPrefix", "prefix", "TextWithNoPrefix")]
    [TestCase("Prefix", "prefix", "")]
    [TestCase("Prefix", "", "Prefix")]
    public void TrimStart_Works_WithOrdinalIgnoreCase(string? text, string? remove, string? expected)
    {
        var result = text.TrimStart(remove, StringComparison.OrdinalIgnoreCase);
        result.Should().Be(expected);
    }


    [TestCase(null, "Suffix", null)]
    [TestCase("TextSuffix", null, "TextSuffix")]
    [TestCase("", "Suffix", "")]
    [TestCase("", "", "")]
    [TestCase("TextSuffix", "Suffix", "Text")]
    [TestCase("TextWithNo", "Suffix", "TextWithNo")]
    [TestCase("Suffix", "Suffix", "")]
    [TestCase("Suffix", "", "Suffix")]
    public void TrimEnd_Works_WithOrdinal(string? text, string? remove, string? expected)
    {
        var result = text.TrimEnd(remove);
        result.Should().Be(expected);
    }


    [TestCase(null, "suffix", null)]
    [TestCase("TextSuffix", null, "TextSuffix")]
    [TestCase("", "suffix", "")]
    [TestCase("", "", "")]
    [TestCase("TextSuffix", "suffix", "Text")]
    [TestCase("TextWithNo", "suffix", "TextWithNo")]
    [TestCase("Suffix", "suffix", "")]
    [TestCase("Suffix", "", "Suffix")]
    public void TrimEnd_Works_WithOrdinalIgnoreCase(string? text, string? remove, string? expected)
    {
        var result = text.TrimEnd(remove, StringComparison.OrdinalIgnoreCase);
        result.Should().Be(expected);
    }

}
