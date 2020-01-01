using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace ToolboxOld.Interop
{
    public class Impersonator : IDisposable
    {


        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(string lpszUsername,
                                            string lpszDomain,
                                            string lpszPassword,
                                            int dwLogonType,
                                            int dwLogonProvider,
                                            ref IntPtr phToken);

        [DllImport("Kernel32")]
        private extern static bool CloseHandle(IntPtr handle);




        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private IntPtr m_Token;
        private readonly WindowsImpersonationContext m_Context = null;
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public Impersonator(string domain, string username, string password)
        {
            m_Token = IntPtr.Zero;
            bool logonSuccessfull = LogonUser(
                username,
                domain,
                password,
                LOGON32_LOGON_INTERACTIVE,
                LOGON32_PROVIDER_DEFAULT,
                ref m_Token);
            if (logonSuccessfull == false)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
            WindowsIdentity identity = new WindowsIdentity(m_Token);
            m_Context = identity.Impersonate();
        }



        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // If disposing equals true, dispose all resources
            if (disposing)
            {
                // Dispose managed resources.
                m_Context.Undo();
                m_Context.Dispose();
            }

            // clean up unmanaged resources here.
            CloseHandle(m_Token);
            m_Token = IntPtr.Zero;
        }



    }



}
