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



    // ReSharper disable NotAccessedPositionalProperty.Local
    private record StableHashTarget(List<int>? Ids = null, List<string>? Tags = null);
    // ReSharper restore NotAccessedPositionalProperty.Local

    [TestCase(new[] { 1, 2, 3 }, new[] { 3, 1, 2 })]
    [TestCase(new[] { 10, 1, 5 }, new[] { 5, 10, 1 })]
    public void ToStableHash_IntListDifferentOrder_SameHash(int[] a, int[] b)
    {
        new StableHashTarget([.. a]).ToStableHash()
            .ShouldBe(new StableHashTarget([.. b]).ToStableHash());
    }

    [TestCase(new[] { "x", "y", "z" }, new[] { "z", "x", "y" })]
    [TestCase(new[] { "banana", "apple" }, new[] { "apple", "banana" })]
    public void ToStableHash_StringListDifferentOrder_SameHash(string[] a, string[] b)
    {
        new StableHashTarget(null, [.. a]).ToStableHash()
            .ShouldBe(new StableHashTarget(null, [.. b]).ToStableHash());
    }

    [TestCase(new[] { 1, 2, 3 }, new[] { 1, 2, 4 })]
    [TestCase(new[] { 1, 2, 3 }, new[] { 1, 2 })]
    public void ToStableHash_DifferentContent_DifferentHash(int[] a, int[] b)
    {
        new StableHashTarget([.. a]).ToStableHash()
            .ShouldNotBe(new StableHashTarget([.. b]).ToStableHash());
    }


}