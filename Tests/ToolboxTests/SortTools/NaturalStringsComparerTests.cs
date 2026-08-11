using aemarcoCommons.Toolbox.SortTools;

namespace ToolboxTests.SortTools;

[TestFixture]
public class NaturalStringsComparerTests
{

    [TestCase("file2.txt", "file10.txt", -1)] //NumericRunSortsByValue
    [TestCase("file10.txt", "file2.txt", 1)]
    [TestCase("file2.txt", "file2.txt", 0)]
    [TestCase("abc", "abd", -1)]
    [TestCase("abc", "ABC", 0)] //CaseInsensitive
    [TestCase("file007.txt", "file7.txt", 0)] //LeadingZerosIgnored
    [TestCase("a", "ab", -1)] //ShorterPrefixSortsFirst
    [TestCase("", "", 0)]
    public void Compare_VariousInputs_ReturnsExpectedSign(string a, string b, int expectedSign)
    {
        var result = Math.Sign(new NaturalStringsComparer().Compare(a, b));
        result.ShouldBe(expectedSign);
    }

    [Test]
    public void Compare_NullVsNull_ReturnsZero()
    {
        new NaturalStringsComparer().Compare(null, null).ShouldBe(0);
    }

    [Test]
    public void Compare_NullVsValue_ReturnsNegative()
    {
        Math.Sign(new NaturalStringsComparer().Compare(null, "a")).ShouldBe(-1);
    }

    [Test]
    public void Compare_ValueVsNull_ReturnsPositive()
    {
        Math.Sign(new NaturalStringsComparer().Compare("a", null)).ShouldBe(1);
    }

    [Test]
    public void Sort_MixedNumericFilenames_OrdersNaturally()
    {
        var files = new[] { "file10.txt", "file2.txt", "file1.txt" };
        Array.Sort(files, new NaturalStringsComparer());
        files.ShouldBe(["file1.txt", "file2.txt", "file10.txt"]);
    }

}
