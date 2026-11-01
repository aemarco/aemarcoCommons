using aemarcoCommons.Toolbox.FileTools.Sniffing;

namespace ToolboxTests.FileTools.Sniffing;

[TestFixture]
internal class MpegTransportStreamSnifferTests
{
    private static FileInfo GetFixture(string name) =>
        TestDataFile.GetFixture("Resources", "TestData", "Ts", name);

    [TestCase("valid.ts", SniffOutcome.Confirmed, null, Description = "Valid TS bytes, correctly named")]
    [TestCase("valid.mp4", SniffOutcome.Corrected, ".ts", Description = "Valid TS bytes, wrongly named as mp4")]
    [TestCase("corrupted.ts", SniffOutcome.Rejected, null, Description = "Sync byte corrupted mid-stream")]
    [TestCase("too-short.ts", SniffOutcome.Rejected, null, Description = "File shorter than one packet")]
    public void Sniff_VariousFixtures_ReturnsExpectedOutcome(string fileName, SniffOutcome expectedOutcome, string? expectedCorrectedExtension)
    {
        var file = GetFixture(fileName);

        var result = new MpegTransportStreamSniffer().Sniff(file);

        result.Outcome.ShouldBe(expectedOutcome);
        result.CorrectedExtension.ShouldBe(expectedCorrectedExtension);
    }
}