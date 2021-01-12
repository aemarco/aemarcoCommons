using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable MemberCanBePrivate.Global

// source info: https://www.renebergelt.de/blog/2018/03/lazy-loaded-properties-in-wpf

namespace aemarcoCommons.WpfTools.BaseModels
{
    public class LazyProperty<T> : INotifyPropertyChanged
    {
        private readonly CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        private readonly Func<CancellationToken, Task<T>> _retrievalFunc;
        private readonly T _defaultValue;

        public LazyProperty(Func<CancellationToken, Task<T>> retrievalFunc, T defaultValue = default)
        {
            _retrievalFunc = retrievalFunc ?? throw new ArgumentNullException(nameof(retrievalFunc));
            _defaultValue = defaultValue;
        }


        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading == value) return;
                _isLoading = value;
                OnPropertyChanged();
            }
        }


        private bool _errorOnLoading;
        public bool ErrorOnLoading
        {
            get => _errorOnLoading;
            set
            {
                if (_errorOnLoading == value) return;
                _errorOnLoading = value;
                OnPropertyChanged();
            }
        }


        private bool _isLoaded;
        private T _value;
        public T Value
        {
            get
            {
                if (_isLoaded) return _value;

                if (_isLoading) return _defaultValue;


                IsLoading = true;
                LoadValueAsync()
                    .ContinueWith((t) =>
                    {
                        if (t.IsCanceled) return;

                        if (t.IsFaulted)
                        {
                            _value = _defaultValue;
                            ErrorOnLoading = true;
                            _isLoaded = true;
                            IsLoading = false;
                            OnPropertyChanged(nameof(Value));
                        }
                        else
                        {
                            Value = t.Result;
                        }
                    });
                return _defaultValue;
            }
            // if you want a ReadOnly-property just set this setter to private
            set
            {
                if (_isLoading)
                    // since we set the value now, there is no need
                    // to retrieve the "old" value asynchronously
                    CancelLoading();

                if (EqualityComparer<T>.Default.Equals(_value, value)) return;

                _value = value;
                _isLoaded = true;
                IsLoading = false;
                ErrorOnLoading = false;

                OnPropertyChanged();
            }
        }

        private async Task<T> LoadValueAsync()
        {
            return await _retrievalFunc(_cancelTokenSource.Token);
        }

        public void CancelLoading()
        {
            _cancelTokenSource.Cancel();
        }


        /// <summary>
        /// This allows you to assign the value of this lazy property directly
        /// to a variable of type T
        /// </summary>        
        public static implicit operator T(LazyProperty<T> p)
        {
            return p.Value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
