namespace aemarcoCommons.Toolbox.FileTools.Sniffing;

public sealed class UnknownSniffer
{

    private static readonly Lazy<IReadOnlyCollection<IContentSniffer>> AllKnownSniffers = new(DiscoverSniffers);
    private readonly IReadOnlyCollection<IContentSniffer> _sniffers;
    public UnknownSniffer(IReadOnlyCollection<IContentSniffer> sniffers)
    {
        _sniffers = sniffers;
    }
    public UnknownSniffer()
        : this(AllKnownSniffers.Value)
    { }


    public SniffResult Sniff(FileInfo file) =>
        Sniff(
            file,
            [
                .. _sniffers
                    .OrderByDescending(s => s.Extension.Equals(file.Extension, StringComparison.OrdinalIgnoreCase))
            ]);


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

    private static IReadOnlyCollection<IContentSniffer> DiscoverSniffers() =>
    [
        .. typeof(IContentSniffer).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } &&
                        typeof(IContentSniffer).IsAssignableFrom(t) &&
                        t.GetConstructor(Type.EmptyTypes) is not null)
            .Select(t => (IContentSniffer)Activator.CreateInstance(t)!)
    ];

}