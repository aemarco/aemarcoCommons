using aemarcoCommons.Toolbox.FileTools.Sniffing;

namespace ToolboxTests.FileTools.Sniffing;

[TestFixture]
internal class UnknownSnifferTests
{
    private static UnknownSniffer CreateSniffer() =>
        new([new MpegTransportStreamSniffer(), new AvifSniffer()]);

    [TestCase("Ts", "valid.mp4", SniffOutcome.Corrected, ".ts", Description = "TS bytes wrongly named as mp4, identified via MpegTransportStreamSniffer")]
    [TestCase("Avif", "cloaked.jpg", SniffOutcome.Corrected, ".avif", Description = "AVIF bytes cloaked as jpg, identified via AvifSniffer")]
    [TestCase("Ts", "too-short.ts", SniffOutcome.Rejected, null, Description = "Matches no known sniffer")]
    public void Sniff_VariousFixtures_ReturnsExpectedOutcome(string subfolder, string fileName, SniffOutcome expectedOutcome, string? expectedCorrectedExtension)
    {
        var file = TestDataFile.GetFixture("Resources", "TestData", subfolder, fileName);

        var result = CreateSniffer().Sniff(file);

        result.Outcome.ShouldBe(expectedOutcome);
        result.CorrectedExtension.ShouldBe(expectedCorrectedExtension);
    }

    [TestCase("empty.avif", SniffOutcome.Confirmed, Description = "Already-correct extension recognized as-is")]
    public void Sniff_RealEmptyFilesFixture_ReturnsExpectedOutcome(string fileName, SniffOutcome expectedOutcome)
    {
        var file = TestDataFile.GetEmptyFilesFixture("image", fileName);

        var result = CreateSniffer().Sniff(file);

        result.Outcome.ShouldBe(expectedOutcome);
        result.CorrectedExtension.ShouldBeNull();
    }
}
