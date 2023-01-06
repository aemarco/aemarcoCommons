using aemarcoCommons.Extensions.CryptoExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace ExtensionsTests.CryptoExtensionsTests;

public class HexStuffTests
{


    [TestCase("test", "098F6BCD4621D373CADE4E832627B4F6")]
    [TestCase("oMyDear", "77B80F6DE365762B070048D1F59ABB39")]
    public void ToHexHashString_Returns_Correctly(string text, string expected)
    {

        var result = text.ToHexHashString();
        result.Should().Be(expected);
    }



}