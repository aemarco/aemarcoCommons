using aemarcoCommons.Toolbox.FileTools.Sniffing;

namespace ToolboxTests.FileTools.Sniffing;

[TestFixture]
internal class AvifSnifferTests
{
    private static FileInfo GetFixture(string name) =>
        TestDataFile.GetFixture("Resources", "TestData", "Avif", name);

    [TestCase("empty.avif", SniffOutcome.Confirmed, Description = "Real AVIF fixture")]
    [TestCase("empty.jpg", SniffOutcome.Rejected, Description = "Real JPEG fixture, not AVIF")]
    public void Sniff_RealEmptyFilesFixtures_ReturnsExpectedOutcome(string fileName, SniffOutcome expectedOutcome)
    {
        var file = TestDataFile.GetEmptyFilesFixture("image", fileName);

        var result = new AvifSniffer().Sniff(file);

        result.Outcome.ShouldBe(expectedOutcome);
        result.CorrectedExtension.ShouldBeNull();
    }

    [TestCase("cloaked.jpg", SniffOutcome.Corrected, ".avif", Description = "AVIF bytes cloaked as jpg")]
    [TestCase("avis-cloaked.png", SniffOutcome.Corrected, ".avif", Description = "AVIF sequence bytes cloaked as png")]
    [TestCase("too-short.avif", SniffOutcome.Rejected, null, Description = "File shorter than the ftyp header")]
    public void Sniff_VariousFixtures_ReturnsExpectedOutcome(string fileName, SniffOutcome expectedOutcome, string? expectedCorrectedExtension)
    {
        var file = GetFixture(fileName);

        var result = new AvifSniffer().Sniff(file);

        result.Outcome.ShouldBe(expectedOutcome);
        result.CorrectedExtension.ShouldBe(expectedCorrectedExtension);
    }
}
