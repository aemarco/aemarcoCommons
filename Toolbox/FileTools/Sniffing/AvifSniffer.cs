namespace aemarcoCommons.Toolbox.FileTools.Sniffing;

public sealed class AvifSniffer : IContentSniffer
{
    private const int MinHeaderLength = 16;
    private const int MaxBytesToRead = 4096;
    private static ReadOnlySpan<byte> FtypMarker => "ftyp"u8;
    private static ReadOnlySpan<byte> AvifBrand => "avif"u8;
    private static ReadOnlySpan<byte> AvisBrand => "avis"u8;

    public string Extension => ".avif";

    public SniffResult Sniff(FileInfo file)
    {
        if (file.Length < MinHeaderLength)
            return SniffResult.Rejected();

        var toRead = (int)Math.Min(file.Length, MaxBytesToRead);
        using var stream = file.OpenRead();
        var buffer = new byte[toRead];
        stream.ReadExactly(buffer);

        if (!buffer.AsSpan(4, 4).SequenceEqual(FtypMarker))
            return SniffResult.Rejected();

        var boxSize = BinaryPrimitives.ReadUInt32BigEndian(buffer);
        var scanLength = (int)Math.Min(boxSize, toRead);

        var isAvif = false;
        for (var offset = 8; offset + 4 <= scanLength; offset += 4)
        {
            var brand = buffer.AsSpan(offset, 4);
            if (brand.SequenceEqual(AvifBrand) || brand.SequenceEqual(AvisBrand))
            {
                isAvif = true;
                break;
            }
        }

        if (!isAvif)
            return SniffResult.Rejected();

        return file.Extension.Equals(Extension, StringComparison.OrdinalIgnoreCase)
            ? SniffResult.Confirmed()
            : SniffResult.CorrectedTo(Extension);
    }
}