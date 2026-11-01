namespace aemarcoCommons.Toolbox.SortTools;

public sealed class NaturalStringsComparer : IComparer<string>
{
    public int Compare(string? a, string? b)
    {
        if (ReferenceEquals(a, b)) return 0;
        if (a is null) return -1;
        if (b is null) return 1;

        var i = 0;
        var j = 0;
        while (i < a.Length && j < b.Length)
        {
            if (char.IsDigit(a[i]) && char.IsDigit(b[j]))
            {
                var digitResult = CompareDigitRuns(a, ref i, b, ref j);
                if (digitResult != 0) return digitResult;
                continue;
            }

            var ca = char.ToUpperInvariant(a[i]);
            var cb = char.ToUpperInvariant(b[j]);
            if (ca != cb) return ca.CompareTo(cb);
            i++;
            j++;
        }

        return (a.Length - i) - (b.Length - j);
    }

    private static int CompareDigitRuns(string a, ref int i, string b, ref int j)
    {
        var startI = i;
        while (i < a.Length && char.IsDigit(a[i])) i++;
        var startJ = j;
        while (j < b.Length && char.IsDigit(b[j])) j++;

        var spanA = a.AsSpan(startI, i - startI).TrimStart('0');
        var spanB = b.AsSpan(startJ, j - startJ).TrimStart('0');

        if (spanA.Length != spanB.Length) return spanA.Length - spanB.Length;
        return spanA.CompareTo(spanB, StringComparison.Ordinal);
    }
}

public sealed class NaturalFileInfosNameComparer : IComparer<FileInfo>
{
    private static readonly NaturalStringsComparer Comparer = new();
    public int Compare(FileInfo? a, FileInfo? b)
    {
        return Comparer.Compare(a?.Name, b?.Name);
    }
}