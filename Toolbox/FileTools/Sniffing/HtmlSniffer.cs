using System.Text;

namespace aemarcoCommons.Toolbox.FileTools.Sniffing;

public sealed class HtmlSniffer : IContentSniffer
{
    private const int HeaderLength = 512;
    private static readonly string[] Markers = ["<!doctype html", "<html"];
    private static readonly HashSet<string> KnownExtensions = new(StringComparer.OrdinalIgnoreCase) { ".html", ".htm" };

    public string Extension => ".html";

    public SniffResult Sniff(FileInfo file)
    {
        if (file.Length == 0)
            return SniffResult.Rejected();

        var toRead = (int)Math.Min(file.Length, HeaderLength);
        using var stream = file.OpenRead();
        var buffer = new byte[toRead];
        stream.ReadExactly(buffer);

        var text = Encoding.UTF8.GetString(buffer).TrimStart(' ', '\t', '\r', '\n');
        if (!Markers.Any(marker => text.StartsWith(marker, StringComparison.OrdinalIgnoreCase)))
            return SniffResult.Rejected();

        return KnownExtensions.Contains(file.Extension)
            ? SniffResult.Confirmed()
            : SniffResult.CorrectedTo(Extension);
    }
}
