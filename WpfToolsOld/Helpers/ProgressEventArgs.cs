using System;

namespace WpfToolsOld.Helpers
{
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(double percent, int done, int total)
        {
            Total = total;
            Done = done;
            Percent = percent;
        }

        public double Percent { get; }
        public int Done { get; }
        public int Total { get; }
    }
}
