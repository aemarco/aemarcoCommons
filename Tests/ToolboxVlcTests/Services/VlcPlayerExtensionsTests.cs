namespace ToolboxVlcTests.Services;

[TestFixture]
internal class VlcPlayerExtensionsTests
{

    [Test]
    [Explicit]
    public void GetVlcFolder()
    {
        var result = VlcPlayerExtensions.GetVlcFolder();
        result.ShouldBe(@"C:\Program Files\VideoLAN\VLC");
    }








}
