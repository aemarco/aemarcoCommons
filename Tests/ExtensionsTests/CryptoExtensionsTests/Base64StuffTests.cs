using aemarcoCommons.Extensions.CryptoExtensions;

namespace ExtensionsTests.CryptoExtensionsTests;

public class Base64StuffTests
{

    [TestCase("test", "CY9rzUYh03PK3k6DJie09g==")]
    [TestCase("oMyDear", "d7gPbeNldisHAEjR9Zq7OQ==")]
    public void ToBase64HashString_Returns_Correctly(string text, string expected)
    {

        var result = text.ToBase64HashString();
        result.ShouldBe(expected);
    }


}