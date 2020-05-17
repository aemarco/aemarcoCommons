using System;

namespace WpfTools.Helpers
{
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(double percent, int done, int total)
        {
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

        public double Percent => Math.Max(0, Math.Min(100, 100.0 * Done / Total));
    }
}
