using System;

namespace aemarcoCommons.WpfTools.Helpers;

public class ProgressEventArgs : EventArgs
{
    private readonly double? _percent;
    public ProgressEventArgs(double percent, int done, int total)
    {
        _percent = percent;
        Total = total;
        Done = done;
    }

    public ProgressEventArgs(int done, int total)
    {
        Done = done;
        Total = total;
    }
    public int Done { get; }
    public int Total { get; }

    public double Percent => _percent ?? Math.Max(0, Math.Min(100, 100.0 * Done / Total));
}