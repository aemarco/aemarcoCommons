using aemarcoCommons.ToolboxImage.Extensions;

namespace ToolboxImageTests.Extensions;

[TestFixture]
internal class ImageExtensionsTests
{

    [TestCase("empty.tif", true, Description = "Real TIFF fixture")]
    [TestCase("empty.jpg", false, Description = "Real JPEG fixture")]
    [TestCase("empty.png", false, Description = "Real PNG fixture")]
    [TestCase("empty.avif", false, Description = "Format ImageSharp does not know")]
    public void IsTiff_RealEmptyFilesFixtures_ReturnsExpected(string fileName, bool expected)
    {
        var file = TestDataFile.GetEmptyFilesFixture("image", fileName);

        var result = file.IsTiff();

        result.ShouldBe(expected);
    }

    [TestCase("empty.tif", true, Description = "Real TIFF fixture")]
    [TestCase("empty.jpg", false, Description = "Real JPEG fixture")]
    [TestCase("empty.avif", false, Description = "Format ImageSharp does not know")]
    public void IsTiff_RealEmptyFilesFixtureBytes_ReturnsExpected(string fileName, bool expected)
    {
        var bytes = File.ReadAllBytes(TestDataFile.GetEmptyFilesFixture("image", fileName).FullName);

        var result = bytes.IsTiff();

        result.ShouldBe(expected);
    }

    [Test]
    public void IsTiff_BytesNotAnImage_ReturnsFalse()
    {
        byte[] bytes = [1, 2, 3, 4, 5, 6, 7, 8];

        var result = bytes.IsTiff();

        result.ShouldBeFalse();
    }
}
