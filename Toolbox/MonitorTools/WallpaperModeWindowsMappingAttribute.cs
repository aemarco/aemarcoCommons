using System;
using Toolbox.Interop;

namespace Toolbox.MonitorTools
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