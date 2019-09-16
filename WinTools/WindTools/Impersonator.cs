namespace Tools.WinTools
{

    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Security.Principal;

    public class Impersonator : IDisposable
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain,
        String lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private const int LOGON32_LOGON_INTERACTIVE = 2;
        readonly IntPtr m_Token;

        private WindowsImpersonationContext m_Context = null;


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
            m_Context.Undo();
            CloseHandle(m_Token);
        }


    }
}
