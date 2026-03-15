using aemarcoCommons.Extensions.MonadExtensions;

namespace ExtensionsTests.MonadExtensionTests;

public class MappingTests
{
    [Test]
    public void Map_Maps()
    {
        var r = 5.Map(x => x * 2);
        r.ShouldBe(10);
    }






}