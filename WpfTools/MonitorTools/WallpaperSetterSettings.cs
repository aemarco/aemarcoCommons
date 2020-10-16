using System;
using System.IO;
using aemarcoCommons.Toolbox.MonitorTools;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace aemarcoCommons.WpfTools.MonitorTools
{
    public interface IWallpaperSetterSettings : IWallpaperRealEstateSettings
    {
        string CombinedWallpaperFilePath { get; }
        string VirtualWallpaperFilePath { get; }
        string LockScreenFilePath { get; }

    }

    public class WallpaperSetterSettings : ISingleton, IWallpaperSetterSettings
    {
        /// <summary>
        /// path to store the Wallpaper, defaults to "CurrentCombinedWallpaper.jpg"
        /// </summary>
        public string CombinedWallpaperFilePath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "CurrentCombinedWallpaper.jpg");

        /// <summary>
        /// path to store the virtual Wallpaper, defaults to "CurrentVirtualWallpaper.jpg"
        /// </summary>
        public string VirtualWallpaperFilePath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "CurrentVirtualWallpaper.jpg");

        /// <summary>
        /// path to store the LockScreen, defaults to "CurrentLockScreen.jpg"
        /// </summary>
        /// 
        public string LockScreenFilePath { get; set; }  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "CurrentLockScreen.jpg");
    
        /// <summary>
        /// defaults to AllowFillForceCut
        ///  Fit: Places the Wallpaper as big as possible without cutting (black bars)
        ///  Fill: Cuts as much needed to fill the screen
        ///  AllowFill: Decides automatically between Fill and Fit based on allowed cutting
        ///  AllowFillForceCut (default): Like AllowFill, otherwise Fit with allowed cutting 
        /// </summary>
        public WallpaperMode WallpaperMode { get; set; } = WallpaperMode.AllowFillForceCut;

        /// <summary>
        /// Defines how many percent that pictures can be cut vertically, defaults to 10
        /// </summary>
        public int PercentTopBottomCutAllowed { get; set; } = 10;

        /// <summary>
        /// Defines how many percent that pictures can be cut horizontally, defaults to 20
        /// </summary>
        public int PercentLeftRightCutAllowed { get; set; } = 20;
    } 
}