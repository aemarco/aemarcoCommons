using aemarcoCommons.Toolbox.Mime;

namespace ToolboxTests.Mime;

[TestFixture]
public class MimeMapTests
{

    [TestCase(".jpg", true, "image/jpeg")]
    [TestCase("jpg", true, "image/jpeg")]
    [TestCase(".txt", true, "text/plain")]
    [TestCase("txt", true, "text/plain")]
    [TestCase(".unknownext", false, "application/octet-stream")]
    [TestCase("png", true, "image/png")]
    [TestCase("file.jpg?param=1", true, "image/jpeg")] //RemovesQuestionMark
    [TestCase("archive.tar.gz", true, "application/x-gzip")] //HandlesMultipleDots
    public void TryGetMimeType_WorksAsExpected(string ext, bool expectedSuccess, string expectedMime)
    {
        var success = MimeMap.TryGetMimeType(ext, out var mime);
        success.ShouldBe(expectedSuccess);
        if (success)
            mime.ShouldBe(expectedMime);
    }
    [Test]
    public void TryGetMimeType_ThrowsOnNull()
    {
        Should.Throw<ArgumentNullException>(
            () => MimeMap.TryGetMimeType(null!, out _));
    }


    [TestCase(".jpg", "image/jpeg")]
    [TestCase("jpg", "image/jpeg")]
    [TestCase(".txt", "text/plain")]
    [TestCase("txt", "text/plain")]
    [TestCase(".unknownext", "application/octet-stream")]
    public void GetMimeType_ReturnsExpectedMimeType(string ext, string expectedMime)
    {
        var result = MimeMap.GetMimeType(ext);
        result.ShouldBe(expectedMime);
    }
    [Test]
    public void GetMimeType_ThrowsOnNull()
    {
        Should.Throw<ArgumentNullException>(
            () => MimeMap.GetMimeType(null!));
    }


    [TestCase("image/jpeg", ".jpg")]
    [TestCase("text/plain", ".txt")]
    [TestCase("application/pdf", ".pdf")]
    [TestCase("application/unknown", null)]
    public void GetExtension_ReturnsExpectedExtension(string mimeType, string? expectedExt)
    {
        var result = MimeMap.GetExtension(mimeType);
        result.ShouldBe(expectedExt);
    }
    [Test]
    public void GetExtension_ThrowsOnNull()
    {
        Should.Throw<ArgumentNullException>(
            () => MimeMap.GetExtension(null!));
    }
    [Test]
    public void GetExtension_ThrowsOnDotInput()
    {
        Should.Throw<ArgumentException>(
            () => MimeMap.GetExtension(".jpg"));
    }

}