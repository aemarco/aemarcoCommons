using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
#pragma warning disable IDE0130

namespace aemarcoCommons.Toolbox.SortTools;

[SuppressUnmanagedCodeSecurity]
[SupportedOSPlatform("windows")]
internal static class SafeNativeMethods
{
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    public static extern int StrCmpLogicalW(string? psz1, string? psz2);
}

[SupportedOSPlatform("windows")]
[Obsolete("Use aemarcoCommons.Toolbox.SortTools.NaturalStringsComparer instead.")]
public sealed class NaturalStringComparer : IComparer<string>
{
    public int Compare(string? a, string? b)
    {
        return SafeNativeMethods.StrCmpLogicalW(a, b);
    }
}

[SupportedOSPlatform("windows")]
[Obsolete("Use aemarcoCommons.Toolbox.SortTools.NaturalFileInfosNameComparer instead.")]
public sealed class NaturalFileInfoNameComparer : IComparer<FileInfo>
{
    public int Compare(FileInfo? a, FileInfo? b)
    {
        return SafeNativeMethods.StrCmpLogicalW(a?.Name, b?.Name);
    }
}
