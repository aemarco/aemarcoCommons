using aemarcoCommons.Extensions.MonadExtensions;
using FluentAssertions;
using NUnit.Framework;

namespace ExtensionsTests.MonadExtensionTests;

public class MappingTests
{
    [Test]
    public void Map_Maps()
    {
        var r = 5.Map(x => x * 2);
        r.Should().Be(10);
    }






}