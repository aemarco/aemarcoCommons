using System;
using aemarcoCommons.Toolbox.Interop;

namespace aemarcoCommons.Toolbox.MonitorTools
{
    public sealed class WallpaperModeWindowsMappingAttribute : Attribute
    {
        public WallpaperModeWindowsMappingAttribute(WindowsWallpaperStyle windowsWallpaperStyle)
        {
            WindowsWallpaperStyle = windowsWallpaperStyle;
        }
        public WindowsWallpaperStyle WindowsWallpaperStyle { get; }
    }
}