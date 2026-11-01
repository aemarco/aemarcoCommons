namespace aemarcoCommons.Toolbox.FileTools.Sniffing;

public sealed record SniffResult(SniffOutcome Outcome, string? CorrectedExtension = null)
{
    public static SniffResult Confirmed() => new(SniffOutcome.Confirmed);
    public static SniffResult Rejected() => new(SniffOutcome.Rejected);
    public static SniffResult CorrectedTo(string extension) => new(SniffOutcome.Corrected, extension);
}