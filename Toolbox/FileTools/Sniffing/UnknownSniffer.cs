namespace aemarcoCommons.Toolbox.FileTools.Sniffing;

public sealed class UnknownSniffer : IContentSniffer
{
    private readonly IReadOnlyCollection<IContentSniffer> _sniffers;

    public UnknownSniffer(IReadOnlyCollection<IContentSniffer> sniffers)
    {
        _sniffers = sniffers;
    }

    public string Extension =>
        throw new NotSupportedException($"{nameof(UnknownSniffer)} has no fixed extension; call {nameof(Sniff)} directly.");

    public SniffResult Sniff(FileInfo file) => Sniff(file, [.. _sniffers]);

    private static SniffResult Sniff(FileInfo file, IContentSniffer[] remaining)
    {
        if (remaining.Length == 0)
            return SniffResult.Rejected();

        var result = remaining[0].Sniff(file);
        return result.Outcome switch
        {
            SniffOutcome.Rejected => Sniff(file, remaining[1..]),
            _ => result,
        };
    }
}
