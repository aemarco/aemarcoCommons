using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
// ReSharper disable All
// author and credits to https://www.codeproject.com/Articles/2939/Network-Shares-and-UNC-paths

namespace aemarcoCommons.ToolboxConsole.Shares;

#region Share Type

/// <summary>
/// Type of share
/// </summary>
[Flags]
public enum ShareType
{
    /// <summary>Disk share</summary>
    Disk = 0,
    /// <summary>Printer share</summary>
    Printer = 1,
    /// <summary>Device share</summary>
    Device = 2,
    /// <summary>IPC share</summary>
    IPC = 3,
    /// <summary>Special share</summary>
    Special = -2147483648, // 0x80000000,
}

#endregion

#region Share

/// <summary>
/// Information about a local share
/// </summary>
public class Share
{
    #region Private data

    private readonly string _server;
    private readonly string _netName;
    private readonly string _path;
    private readonly ShareType _shareType;
    private readonly string _remark;

    #endregion

    #region Constructor

    public Share(string server, string netName, string path, ShareType shareType, string remark)
    {
        if (ShareType.Special == shareType && "IPC$" == netName)
        {
            shareType |= ShareType.IPC;
        }

        _server = server;
        _netName = netName;
        _path = path;
        _shareType = shareType;
        _remark = remark;
    }

    #endregion

    #region Properties

    public string Server => _server;
    public string NetName => _netName;
    public string Path => _path;
    public ShareType ShareType => _shareType;
    public string Remark => _remark;

    /// <summary>
    /// Returns true if this is a file system share
    /// </summary>
    public bool IsFileSystem
    {
        get
        {
            if (0 != (_shareType & ShareType.Device)) return false;
            if (0 != (_shareType & ShareType.IPC)) return false;
            if (0 != (_shareType & ShareType.Printer)) return false;
            if (0 == (_shareType & ShareType.Special)) return true;
            if (ShareType.Special == _shareType && null != _netName && 0 != _netName.Length)
                return true;
            else
                return false;
        }
    }

    /// <summary>
    /// Get the root of a disk-based share
    /// </summary>
    public DirectoryInfo? Root
    {
        get
        {
            if (IsFileSystem)
            {
                if (null == _server || 0 == _server.Length)
                    if (null == _path || 0 == _path.Length)
                        return new DirectoryInfo(ToString());
                    else
                        return new DirectoryInfo(_path);
                else
                    return new DirectoryInfo(ToString());
            }
            else
                return null;
        }
    }

    #endregion

    public override string ToString()
    {
        if (null == _server || 0 == _server.Length)
            return string.Format(@"\\{0}\{1}", Environment.MachineName, _netName);
        else
            return string.Format(@"\\{0}\{1}", _server, _netName);
    }

    public bool MatchesPath(string? path)
    {
        if (!IsFileSystem) return false;
        if (null == path || 0 == path.Length) return true;
        return path.ToLower().StartsWith(_path.ToLower());
    }
}

#endregion

#region Share Collection

/// <summary>
/// A collection of shares
/// </summary>
public class ShareCollection : ReadOnlyCollectionBase
{
    #region Platform

    protected static bool IsNT => PlatformID.Win32NT == Environment.OSVersion.Platform;

    protected static bool IsW2KUp
    {
        get
        {
            OperatingSystem os = Environment.OSVersion;
            return PlatformID.Win32NT == os.Platform && os.Version.Major >= 5;
        }
    }

    #endregion

    #region Interop

    #region Constants

    protected const int MAX_PATH = 260;
    protected const int NO_ERROR = 0;
    protected const int ERROR_ACCESS_DENIED = 5;
    protected const int ERROR_WRONG_LEVEL = 124;
    protected const int ERROR_MORE_DATA = 234;
    protected const int ERROR_NOT_CONNECTED = 2250;
    protected const int UNIVERSAL_NAME_INFO_LEVEL = 1;
    protected const int MAX_SI50_ENTRIES = 20;

    #endregion

    #region Structures

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    protected struct UNIVERSAL_NAME_INFO
    {
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpUniversalName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    protected struct SHARE_INFO_2
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string NetName;
        public ShareType ShareType;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Remark;
        public int Permissions;
        public int MaxUsers;
        public int CurrentUsers;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Path;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Password;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    protected struct SHARE_INFO_1
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string NetName;
        public ShareType ShareType;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Remark;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    protected struct SHARE_INFO_50
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string NetName;
        public byte bShareType;
        public ushort Flags;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Remark;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Path;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string PasswordRW;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string PasswordRO;
        public ShareType ShareType => (ShareType)((int)bShareType & 0x7F);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    protected struct SHARE_INFO_1_9x
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string NetName;
        public byte Padding;
        public ushort bShareType;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Remark;
        public ShareType ShareType => (ShareType)((int)bShareType & 0x7FFF);
    }

    #endregion

    #region Functions

    [DllImport("mpr", CharSet = CharSet.Auto)]
    protected static extern int WNetGetUniversalName(string lpLocalPath,
        int dwInfoLevel, ref UNIVERSAL_NAME_INFO lpBuffer, ref int lpBufferSize);

    [DllImport("mpr", CharSet = CharSet.Auto)]
    protected static extern int WNetGetUniversalName(string lpLocalPath,
        int dwInfoLevel, IntPtr lpBuffer, ref int lpBufferSize);

    [DllImport("netapi32", CharSet = CharSet.Unicode)]
    protected static extern int NetShareEnum(string? lpServerName, int dwLevel,
        out IntPtr lpBuffer, int dwPrefMaxLen, out int entriesRead,
        out int totalEntries, ref int hResume);

    [DllImport("svrapi", CharSet = CharSet.Ansi)]
    protected static extern int NetShareEnum(
        [MarshalAs(UnmanagedType.LPTStr)] string? lpServerName, int dwLevel,
        IntPtr lpBuffer, ushort cbBuffer, out ushort entriesRead,
        out ushort totalEntries);

    [DllImport("netapi32")]
    protected static extern int NetApiBufferFree(IntPtr lpBuffer);

    #endregion

    #region Enumerate shares

    protected static void EnumerateSharesNT(string? server, ShareCollection shares)
    {
        int level = 2;
        IntPtr pBuffer = IntPtr.Zero;

        try
        {
            var hResume = 0;
            var nRet = NetShareEnum(server, level, out pBuffer, -1,
                out var entriesRead, out var totalEntries, ref hResume);

            if (ERROR_ACCESS_DENIED == nRet)
            {
                level = 1;
                nRet = NetShareEnum(server, level, out pBuffer, -1,
                    out entriesRead, out totalEntries, ref hResume);
            }

            if (NO_ERROR == nRet && entriesRead > 0)
            {
                Type t = (2 == level) ? typeof(SHARE_INFO_2) : typeof(SHARE_INFO_1);
                int offset = Marshal.SizeOf(t);

                IntPtr pItem = pBuffer;
                for (int i = 0; i < entriesRead; i++, pItem += offset)
                {
                    if (1 == level)
                    {
                        SHARE_INFO_1 si = (SHARE_INFO_1)Marshal.PtrToStructure(pItem, t)!;
                        shares.Add(si.NetName, string.Empty, si.ShareType, si.Remark);
                    }
                    else
                    {
                        SHARE_INFO_2 si = (SHARE_INFO_2)Marshal.PtrToStructure(pItem, t)!;
                        shares.Add(si.NetName, si.Path, si.ShareType, si.Remark);
                    }
                }
            }
        }
        finally
        {
            if (IntPtr.Zero != pBuffer)
                NetApiBufferFree(pBuffer);
        }
    }

    protected static void EnumerateShares9x(string? server, ShareCollection shares)
    {
        int level = 50;

        Type t = typeof(SHARE_INFO_50);
        int size = Marshal.SizeOf(t);
        ushort cbBuffer = (ushort)(MAX_SI50_ENTRIES * size);
        IntPtr pBuffer = Marshal.AllocHGlobal(cbBuffer);

        try
        {
            var nRet = NetShareEnum(server, level, pBuffer, cbBuffer,
                out var entriesRead, out var totalEntries);

            if (ERROR_WRONG_LEVEL == nRet)
            {
                level = 1;
                t = typeof(SHARE_INFO_1_9x);
                size = Marshal.SizeOf(t);

                nRet = NetShareEnum(server, level, pBuffer, cbBuffer,
                    out entriesRead, out totalEntries);
            }

            if (NO_ERROR == nRet || ERROR_MORE_DATA == nRet)
            {
                for (long i = 0, lpItem = pBuffer.ToInt64(); i < entriesRead; i++, lpItem += size)
                {
                    IntPtr pItem = new IntPtr(lpItem);

                    if (1 == level)
                    {
                        SHARE_INFO_1_9x si = (SHARE_INFO_1_9x)Marshal.PtrToStructure(pItem, t)!;
                        shares.Add(si.NetName, string.Empty, si.ShareType, si.Remark);
                    }
                    else
                    {
                        SHARE_INFO_50 si = (SHARE_INFO_50)Marshal.PtrToStructure(pItem, t)!;
                        shares.Add(si.NetName, si.Path, si.ShareType, si.Remark);
                    }
                }
            }
            else
                Console.WriteLine(nRet);
        }
        finally
        {
            Marshal.FreeHGlobal(pBuffer);
        }
    }

    protected static void EnumerateShares(string server, ShareCollection shares)
    {
        if (null != server && 0 != server.Length && !IsW2KUp)
        {
            server = server.ToUpper();
            if (!('\\' == server[0] && '\\' == server[1]))
                server = @"\\" + server;
        }

        if (IsNT)
            EnumerateSharesNT(server, shares);
        else
            EnumerateShares9x(server, shares);
    }

    #endregion

    #endregion

    #region Static methods

    public static bool IsValidFilePath(string fileName)
    {
        if (null == fileName || 0 == fileName.Length) return false;

        char drive = char.ToUpper(fileName[0]);
        if ('A' > drive || drive > 'Z') return false;
        else if (Path.VolumeSeparatorChar != fileName[1]) return false;
        else if (Path.DirectorySeparatorChar != fileName[2]) return false;
        else return true;
    }

    public static string PathToUnc(string fileName)
    {
        if (null == fileName || 0 == fileName.Length) return string.Empty;

        fileName = Path.GetFullPath(fileName);
        if (!IsValidFilePath(fileName)) return fileName;

        UNIVERSAL_NAME_INFO rni = new UNIVERSAL_NAME_INFO();
        int bufferSize = Marshal.SizeOf(rni);

        var nRet = WNetGetUniversalName(fileName, UNIVERSAL_NAME_INFO_LEVEL, ref rni, ref bufferSize);

        if (ERROR_MORE_DATA == nRet)
        {
            IntPtr pBuffer = Marshal.AllocHGlobal(bufferSize);
            try
            {
                nRet = WNetGetUniversalName(fileName, UNIVERSAL_NAME_INFO_LEVEL, pBuffer, ref bufferSize);
                if (NO_ERROR == nRet)
                    rni = (UNIVERSAL_NAME_INFO)Marshal.PtrToStructure(pBuffer, typeof(UNIVERSAL_NAME_INFO))!;
            }
            finally
            {
                Marshal.FreeHGlobal(pBuffer);
            }
        }

        switch (nRet)
        {
            case NO_ERROR:
                return rni.lpUniversalName;
            case ERROR_NOT_CONNECTED:
                ShareCollection shi = LocalShares;
                if (null != shi)
                {
                    Share? share = shi[fileName];
                    if (null != share)
                    {
                        string path = share.Path;
                        if (null != path && 0 != path.Length)
                        {
                            int index = path.Length;
                            if (Path.DirectorySeparatorChar != path[path.Length - 1])
                                index++;
                            if (index < fileName.Length)
                                fileName = fileName.Substring(index);
                            else
                                fileName = string.Empty;
                            fileName = Path.Combine(share.ToString(), fileName);
                        }
                    }
                }
                return fileName;
            default:
                Console.WriteLine("Unknown return value: {0}", nRet);
                return string.Empty;
        }
    }

    public static Share? PathToShare(string? fileName)
    {
        if (null == fileName || 0 == fileName.Length) return null;
        fileName = Path.GetFullPath(fileName);
        if (!IsValidFilePath(fileName)) return null;
        ShareCollection shi = LocalShares;
        return null == shi ? null : shi[fileName];
    }

    #endregion

    #region Local shares

    private static ShareCollection? _local;

    public static ShareCollection LocalShares => _local ??= new ShareCollection();

    public static ShareCollection GetShares(string server) => new ShareCollection(server);

    #endregion

    #region Private Data

    private readonly string _server;

    #endregion

    #region Constructor

    private ShareCollection()
    {
        _server = string.Empty;
        EnumerateShares(_server, this);
    }

    private ShareCollection(string server)
    {
        _server = server;
        EnumerateShares(_server, this);
    }

    #endregion

    #region Add

    protected void Add(Share share) => InnerList.Add(share);

    protected void Add(string netName, string path, ShareType shareType, string remark) =>
        InnerList.Add(new Share(_server, netName, path, shareType, remark));

    #endregion

    #region Properties

    public string Server => _server;

    public Share this[int index] => (Share)InnerList[index]!;

    public Share? this[string? path]
    {
        get
        {
            if (null == path || 0 == path.Length) return null;
            path = Path.GetFullPath(path);
            if (!IsValidFilePath(path)) return null;

            Share? match = null;
            for (int i = 0; i < InnerList.Count; i++)
            {
                Share s = (Share)InnerList[i]!;
                if (s.IsFileSystem && s.MatchesPath(path))
                {
                    if (null == match)
                        match = s;
                    else if (match.Path.Length < s.Path.Length)
                    {
                        if (ShareType.Disk == s.ShareType || ShareType.Disk != match.ShareType)
                            match = s;
                    }
                }
            }
            return match;
        }
    }

    #endregion

    public void CopyTo(Share[] array, int index) => InnerList.CopyTo(array, index);
}

#endregion
