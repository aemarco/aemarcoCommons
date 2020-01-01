using System;

namespace WpfToolsOld.Helpers
{
    public class ProgressHelper
    {
        private int _countDone;
        readonly int _countTotal;
        readonly EventHandler<ProgressEventArgs> _progress;
        private readonly EventHandler _completed;
        readonly object _caller;

        public ProgressHelper(int countTotal, EventHandler<ProgressEventArgs> progress, EventHandler completed, object caller)
        {
            _countDone = 0;
            _countTotal = countTotal;
            _progress = progress;
            _completed = completed;
            _caller = caller;
        }

        public void ReportProgress()
        {
            _countDone++;
            var progress = 100.0 * _countDone / _countTotal;

            _progress?.Invoke(_caller, new ProgressEventArgs(progress, _countDone, _countTotal));

            if (_countDone == _countTotal)
            {
                _completed?.Invoke(_caller, EventArgs.Empty);
            }
        }


    }
}
