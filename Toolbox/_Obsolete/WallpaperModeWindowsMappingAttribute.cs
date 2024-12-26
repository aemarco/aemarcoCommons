using aemarcoCommons.Toolbox.Interop;
using System;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Toolbox.MonitorTools
{

    [Obsolete]
    public sealed class WallpaperModeWindowsMappingAttribute : Attribute
    {
        public WallpaperModeWindowsMappingAttribute(WindowsWallpaperStyle windowsWallpaperStyle)
        {
            WindowsWallpaperStyle = windowsWallpaperStyle;
        }
        public WindowsWallpaperStyle WindowsWallpaperStyle { get; }
    }
}