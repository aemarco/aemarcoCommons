using System;
using System.Reflection;
using System.Threading;

namespace aemarcoCommons.Toolbox.SyncTools
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class AppSingleStartup : IDisposable
    {

        #region ctor

        /// <summary>
        /// uses provided name
        /// </summary>
        /// <param name="name"></param>
        // ReSharper disable once MemberCanBePrivate.Global
        public AppSingleStartup(string name)
        {
            Name = name;
        }

        /// <summary>
        /// uses Manifest Name
        /// </summary>
        public AppSingleStartup()
            : this(Assembly.GetEntryAssembly()?.ManifestModule.Name)
        { }

        #endregion

        public string Name { get; set; }


        private Mutex _mutex;
        public bool Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AppSingleStartup));

            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Name of starting App needs to be specified.", nameof(Name));


            _mutex = new Mutex(true, Name, out var owned);

            if (!owned) Dispose();
            return owned;
        }

        #region IDisposable

        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                //managed resources here
                _mutex.Dispose();
            }
            //unmanaged resources here

            _disposed = true;
        }


        //needed when unmanaged resources
        ~AppSingleStartup()
        {
            Dispose(false);
        }

        #endregion

    }
}
