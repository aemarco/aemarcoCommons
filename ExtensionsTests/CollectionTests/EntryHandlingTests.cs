using aemarcoCommons.Extensions.CollectionExtensions;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;

namespace ExtensionsTests.CollectionTests;


[TestFixture]
public class EntryHandlingTests
{

    [Test]
    public void ConsolidateRanges_NoOverlap_DoesNotMerge()
    {
        var ranges = new[] { (1, 3), (5, 6) };
        var consolidated = ranges.ConsolidateRanges().ToList();

        consolidated.Should().HaveCount(2);
        consolidated.Should().ContainInOrder(
            (1, 3),
            (5, 6)
        );
    }

    [Test]
    public void ConsolidateRanges_Overlap_Merges()
    {
        var ranges = new[] { (1, 3), (2, 6), (5, 9) };
        var consolidated = ranges.ConsolidateRanges().ToList();

        consolidated.Should().HaveCount(1);
        consolidated.Should().Contain((1, 9));
    }

    [Test]
    public void ConsolidateRanges_Contain_Merges()
    {
        var ranges = new[] { (1, 9), (2, 6), (5, 9) };
        var consolidated = ranges.ConsolidateRanges().ToList();

        consolidated.Should().HaveCount(1);
        consolidated.Should().Contain((1, 9));
    }

    [Test]
    public void ConsolidateRanges_Adjacent_Merges()
    {
        var ranges = new[] { (1, 3), (4, 6), (6, 9) };
        var consolidated = ranges.ConsolidateRanges().ToList();

        consolidated.Should().HaveCount(1);
        consolidated.Should().Contain((1, 9));
    }

    [Test]
    public void ConsolidateRanges_SingleRange_NoChange()
    {
        var ranges = new[] { (1, 3) };
        var consolidated = ranges.ConsolidateRanges().ToList();

        consolidated.Should().HaveCount(1);
        consolidated.Should().Contain((1, 3));
    }

    [Test]
    public void ConsolidateRanges_EmptyInput_NoChange()
    {
        var ranges = Enumerable.Empty<(int, int)>();
        var consolidated = ranges.ConsolidateRanges().ToList();

        consolidated.Should().BeEmpty();
    }
}