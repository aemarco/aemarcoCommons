using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace ToolboxOld.Interop
{
    public class Impersonator : IDisposable
    {

        // ReSharper disable once StringLiteralTypo
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(string lpszUsername,
                                            string lpszDomain,
                                            string lpszPassword,
                                            int dwLogonType,
                                            int dwLogonProvider,
                                            ref IntPtr phToken);

        [DllImport("Kernel32")]
        private static extern bool CloseHandle(IntPtr handle);




        private const int Logon32LogonInteractive = 2;
        private const int Logon32ProviderDefault = 0;
        private IntPtr _mToken;
        private readonly WindowsImpersonationContext _mContext;
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public Impersonator(string domain, string username, string password)
        {
            _mToken = IntPtr.Zero;
            var logonSuccessful = LogonUser(
                username,
                domain,
                password,
                Logon32LogonInteractive,
                Logon32ProviderDefault,
                ref _mToken);
            if (logonSuccessful == false)
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
            var identity = new WindowsIdentity(_mToken);
            _mContext = identity.Impersonate();
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
                _mContext.Undo();
                _mContext.Dispose();
            }

            //unmanaged resources here
            CloseHandle(_mToken);
            _mToken = IntPtr.Zero;

            _disposed = true;
        }

        ~Impersonator()
        {
            Dispose(false);
        }

        #endregion

    }



}
