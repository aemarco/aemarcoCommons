using aemarcoCommons.Toolbox.FileTools.Sniffing;

namespace ToolboxTests.FileTools.Sniffing;

[TestFixture]
internal class HtmlSnifferTests
{
    private static FileInfo GetFixture(string name) =>
        TestDataFile.GetFixture("Resources", "TestData", "Html", name);

    [TestCase("real.html", SniffOutcome.Confirmed, null, Description = "Real HTML content, already .html")]
    [TestCase("real.htm", SniffOutcome.Confirmed, null, Description = "Real HTML content, already .htm")]
    [TestCase("cloaked.jpg", SniffOutcome.Corrected, ".html", Description = "HTML content cloaked as jpg (e.g. a 404 page saved as an image)")]
    [TestCase("upper-cased.jpg", SniffOutcome.Corrected, ".html", Description = "Upper-cased HTML markup cloaked as jpg")]
    [TestCase("not-html.jpg", SniffOutcome.Rejected, null, Description = "Plain text, not HTML")]
    [TestCase("empty.jpg", SniffOutcome.Rejected, null, Description = "Empty file")]
    public void Sniff_VariousFixtures_ReturnsExpectedOutcome(string fileName, SniffOutcome expectedOutcome, string? expectedCorrectedExtension)
    {
        var file = GetFixture(fileName);

        var result = new HtmlSniffer().Sniff(file);

        result.Outcome.ShouldBe(expectedOutcome);
        result.CorrectedExtension.ShouldBe(expectedCorrectedExtension);
    }
}