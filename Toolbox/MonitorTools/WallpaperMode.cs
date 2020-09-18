using aemarcoCommons.Toolbox.Interop;

namespace aemarcoCommons.Toolbox.MonitorTools
{
    public enum WallpaperMode
    {
        [WallpaperModeWindowsMapping(WindowsWallpaperStyle.Fit)]
        Fit,
        [WallpaperModeWindowsMapping(WindowsWallpaperStyle.Fill)]
        AllowFill,
        [WallpaperModeWindowsMapping(WindowsWallpaperStyle.Fill)]
        AllowFillForceCut,
        [WallpaperModeWindowsMapping(WindowsWallpaperStyle.Fill)]
        Fill
    }
}