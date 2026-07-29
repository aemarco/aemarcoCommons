namespace aemarcoCommons.Toolbox.FileTools.Sniffing;

public sealed class MpegTransportStreamSniffer : IContentSniffer
{
    private const int PacketSize = 188;
    private const byte SyncByte = 0x47;
    private const int CheckPacketCount = 4;

    public string Extension => ".ts";

    public SniffResult Sniff(FileInfo file)
    {
        var toRead = (int)Math.Min(file.Length, PacketSize * CheckPacketCount);
        if (toRead < PacketSize)
            return SniffResult.Rejected();

        using var stream = file.OpenRead();
        var buffer = new byte[toRead];
        stream.ReadExactly(buffer);

        var packetCount = toRead / PacketSize;
        for (var i = 0; i < packetCount; i++)
        {
            if (buffer[i * PacketSize] != SyncByte)
                return SniffResult.Rejected();
        }

        return file.Extension.Equals(Extension, StringComparison.OrdinalIgnoreCase)
            ? SniffResult.Confirmed()
            : SniffResult.CorrectedTo(Extension);
    }
}
