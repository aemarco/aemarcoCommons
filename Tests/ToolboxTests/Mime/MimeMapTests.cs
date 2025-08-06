using aemarcoCommons.Toolbox.Mime;
using FluentAssertions;
using NUnit.Framework;

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
        success.Should().Be(expectedSuccess);
        if (success)
            mime.Should().Be(expectedMime);
    }
    [Test]
    public void TryGetMimeType_ThrowsOnNull()
    {
        var act = () => MimeMap.TryGetMimeType(null, out _);
        act.Should().Throw<System.ArgumentNullException>();
    }


    [TestCase(".jpg", "image/jpeg")]
    [TestCase("jpg", "image/jpeg")]
    [TestCase(".txt", "text/plain")]
    [TestCase("txt", "text/plain")]
    [TestCase(".unknownext", "application/octet-stream")]
    public void GetMimeType_ReturnsExpectedMimeType(string ext, string expectedMime)
    {
        var result = MimeMap.GetMimeType(ext);
        result.Should().Be(expectedMime);
    }
    [Test]
    public void GetMimeType_ThrowsOnNull()
    {
        var act = () => MimeMap.GetMimeType(null);
        act.Should().Throw<System.ArgumentNullException>();
    }


    [TestCase("image/jpeg", ".jpg")]
    [TestCase("text/plain", ".txt")]
    [TestCase("application/pdf", ".pdf")]
    [TestCase("application/unknown", null)]
    public void GetExtension_ReturnsExpectedExtension(string mimeType, string expectedExt)
    {
        var result = MimeMap.GetExtension(mimeType);
        result.Should().Be(expectedExt);
    }
    [Test]
    public void GetExtension_ThrowsOnNull()
    {
        var act = () => MimeMap.GetExtension(null);
        act.Should().Throw<System.ArgumentNullException>();
    }
    [Test]
    public void GetExtension_ThrowsOnDotInput()
    {
        var act = () => MimeMap.GetExtension(".jpg");
        act.Should().Throw<System.ArgumentException>();
    }

}