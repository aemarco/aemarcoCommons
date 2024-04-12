using aemarcoCommons.Extensions.TextExtensions;
using FluentAssertions;
using NUnit.Framework;
using System;

// ReSharper disable once CheckNamespace
namespace ExtensionsTests.TextExtensionsTests;

[Obsolete]
public class ManipulationTests
{
    [TestCase(null, "Abc", null)]
    [TestCase("PrefixText", null, "PrefixText")]
    [TestCase("", "Prefix", "")]
    [TestCase("PrefixText", "Prefix", "Text")]
    [TestCase("TextWithNoPrefix", "Prefix", "TextWithNoPrefix")]
    [TestCase("Prefix", "Prefix", "")]
    [TestCase("Prefix", "", "Prefix")]
    [TestCase("", "", "")]
    public void TrimStart_Works(string? text, string? remove, string? expected)
    {
        var result = text.TrimStart(remove);
        result.Should().Be(expected);
    }


    [TestCase(null, "Suffix", null)]
    [TestCase("TextSuffix", null, "TextSuffix")]
    [TestCase("", "Suffix", "")]
    [TestCase("TextSuffix", "Suffix", "Text")]
    [TestCase("TextWithNo", "Suffix", "TextWithNo")]
    [TestCase("Suffix", "Suffix", "")]
    [TestCase("Suffix", "", "Suffix")]
    [TestCase("", "", "")]
    public void TrimEnd_Works(string? text, string? remove, string? expected)
    {
        var result = text.TrimEnd(remove);
        result.Should().Be(expected);
    }



}
