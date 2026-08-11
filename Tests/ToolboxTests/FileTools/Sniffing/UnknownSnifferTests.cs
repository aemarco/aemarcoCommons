using aemarcoCommons.Toolbox.FileTools.Sniffing;

namespace ToolboxTests.FileTools.Sniffing;

[TestFixture]
internal class UnknownSnifferTests
{
    private static readonly FileInfo DummyFile = new("dummy.bin");

    private static IContentSniffer StubSniffer(SniffResult result)
    {
        var sniffer = Substitute.For<IContentSniffer>();
        sniffer.Sniff(Arg.Any<FileInfo>()).Returns(result);
        return sniffer;
    }

    [Test]
    public void Sniff_FirstSnifferConfirms_ReturnsConfirmedWithoutCallingRemaining()
    {
        var first = StubSniffer(SniffResult.Confirmed());
        var second = StubSniffer(SniffResult.Confirmed());
        var sniffer = new UnknownSniffer([first, second]);

        var result = sniffer.Sniff(DummyFile);

        result.Outcome.ShouldBe(SniffOutcome.Confirmed);
        second.DidNotReceive().Sniff(Arg.Any<FileInfo>());
    }

    [Test]
    public void Sniff_FirstRejectsSecondConfirms_ReturnsConfirmed()
    {
        var first = StubSniffer(SniffResult.Rejected());
        var second = StubSniffer(SniffResult.Confirmed());
        var sniffer = new UnknownSniffer([first, second]);

        var result = sniffer.Sniff(DummyFile);

        result.Outcome.ShouldBe(SniffOutcome.Confirmed);
    }

    [Test]
    public void Sniff_FirstRejectsSecondCorrects_ReturnsCorrectedExtension()
    {
        var first = StubSniffer(SniffResult.Rejected());
        var second = StubSniffer(SniffResult.CorrectedTo(".foo"));
        var sniffer = new UnknownSniffer([first, second]);

        var result = sniffer.Sniff(DummyFile);

        result.Outcome.ShouldBe(SniffOutcome.Corrected);
        result.CorrectedExtension.ShouldBe(".foo");
    }

    [Test]
    public void Sniff_AllSniffersReject_ReturnsRejected()
    {
        var sniffer = new UnknownSniffer([StubSniffer(SniffResult.Rejected()), StubSniffer(SniffResult.Rejected())]);

        var result = sniffer.Sniff(DummyFile);

        result.Outcome.ShouldBe(SniffOutcome.Rejected);
    }

    [Test]
    public void Sniff_NoSniffers_ReturnsRejected()
    {
        var sniffer = new UnknownSniffer([]);

        var result = sniffer.Sniff(DummyFile);

        result.Outcome.ShouldBe(SniffOutcome.Rejected);
    }

    [Test]
    public void Sniff_ExtensionMatchesLaterSniffer_TriesMatchingSnifferFirst()
    {
        var nonMatching = Substitute.For<IContentSniffer>();
        nonMatching.Extension.Returns(".foo");
        nonMatching.Sniff(Arg.Any<FileInfo>()).Returns(SniffResult.CorrectedTo(".foo"));

        var matching = Substitute.For<IContentSniffer>();
        matching.Extension.Returns(".bar");
        matching.Sniff(Arg.Any<FileInfo>()).Returns(SniffResult.Confirmed());

        var sniffer = new UnknownSniffer([nonMatching, matching]);
        var file = new FileInfo("target.bar");

        var result = sniffer.Sniff(file);

        result.Outcome.ShouldBe(SniffOutcome.Confirmed);
        nonMatching.DidNotReceive().Sniff(Arg.Any<FileInfo>());
    }

    [Test]
    public void Sniff_ParameterlessConstructor_DiscoversKnownSniffers()
    {
        var sniffer = new UnknownSniffer();
        var file = TestDataFile.GetFixture("Resources", "TestData", "Avif", "cloaked.jpg");

        var result = sniffer.Sniff(file);

        result.Outcome.ShouldBe(SniffOutcome.Corrected);
        result.CorrectedExtension.ShouldBe(".avif");
    }

}