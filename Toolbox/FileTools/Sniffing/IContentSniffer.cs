namespace aemarcoCommons.Toolbox.FileTools.Sniffing;

public interface IContentSniffer
{
    string Extension { get; }
    SniffResult Sniff(FileInfo file);
}
