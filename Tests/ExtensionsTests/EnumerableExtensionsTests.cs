using aemarcoCommons.Extensions;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtensionsTests;

public class EnumerableExtensionsTests
{


    [Test]
    public void Shuffle_Works()
    {
        EnumerableExtensions.Random = new Random(42);
        var list = new List<int> { 1, 2, 3 };
        var expected = new List<int> { 3, 2, 1 };

        var result = list.Shuffle();

        result.Should().BeEquivalentTo(expected);
    }







    [TestCase(null, false)]
    [TestCase(new string[] { }, false)]
    [TestCase(new[] { "bob" }, true)]
    public void NotNullOrEmpty_Delivers(string[] collection, bool expected)
    {

        var result = collection.NotNullOrEmpty();
        result.Should().Be(expected);
    }

    [TestCase(null, true)]
    [TestCase(new string[] { }, true)]
    [TestCase(new[] { "bob" }, false)]
    public void NullOrEmpty_Delivers(string[] collection, bool expected)
    {

        var result = collection.NullOrEmpty();
        result.Should().Be(expected);
    }


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

    [Test]
    public void AddDistinct_Should_Add_Item_To_Collection_If_It_Does_Not_Exist()
    {
        // Arrange
        var collection = new List<int>();

        // Act
        collection.AddDistinct(1);
        collection.AddDistinct(2);
        collection.AddDistinct(1);

        // Assert
        collection.Should().BeEquivalentTo(new[] { 1, 2 });
    }

    [Test]
    public void AddRangeDistinct_Should_Add_Items_To_Collection_If_They_Do_Not_Exist()
    {
        // Arrange
        var collection = new List<int>();

        // Act
        collection.AddRangeDistinct(new[] { 1, 2 });
        collection.AddRangeDistinct(new[] { 3, 2 });
        collection.AddRangeDistinct(new[] { 1, 2 });

        // Assert
        collection.Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }



    #region FindClosestItem

    [Test]
    public void FindClosestItem_SingleItemSequence_ReturnsItem()
    {
        IEnumerable<int> sequence = new List<int> { 42 };
        const int target = 50;
        var result = sequence.FindClosestItem(target);
        result.Should().Be(42);
    }

    [TestCase(0, 30)]
    [TestCase(29, 30)]
    [TestCase(34, 30)]
    [TestCase(39, 40)]
    [TestCase(54, 50)]
    [TestCase(61, 60)]
    public void FindClosestItem_MultipleItemsSequence_ReturnsClosestItem(int target, int expected)
    {
        IEnumerable<int> sequence = new List<int> { 30, 40, 50, 60 };
        var result = sequence.FindClosestItem(target);
        result.Should().Be(expected);
    }

    [Test]
    public void FindClosestItem_EmptySequence_ThrowsArgumentException()
    {
        IEnumerable<int> sequence = new List<int>();
        const int target = 100;
        Action action = () => sequence.FindClosestItem(target);
        action.Should().Throw<ArgumentException>().WithMessage("Sequence is empty");
    }

    [Test]
    public void FindClosestItem_NullSequence_ThrowsArgumentNullException()
    {
        IEnumerable<int> sequence = null!;
        const int target = 100;
        Action action = () => sequence.FindClosestItem(target);
        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'sequence')");
    }

    #endregion













}