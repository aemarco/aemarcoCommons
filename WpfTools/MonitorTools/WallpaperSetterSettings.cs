using System;
using System.IO;
using aemarcoCommons.Toolbox.MonitorTools;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace aemarcoCommons.WpfTools.MonitorTools
{
    public interface IWallpaperSetterSettings
    {
        string WallpaperFilePath { get; }
        WallpaperMode WallpaperMode { get; set; }
    }

    public class WallpaperSetterSettings : ISingleton, IWallpaperSetterSettings
    {
        /// <summary>
        /// path to store the wallpaper, defaults to "CurrentWallpaper.jpg"
        /// </summary>
        public string WallpaperFilePath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "CurrentWallpaper.jpg");

        /// <summary>
        /// defaults to AllowFillForceCut
        ///  Fit: Places the Wallpaper as big as possible without cutting (black bars)
        ///  Fill: Cuts as much needed to fill the screen
        ///  AllowFill: Decides automatically between Fill and Fit based on allowed cutting
        ///  AllowFillForceCut (default): Like AllowFill, otherwise Fit with allowed cutting 
        /// </summary>
        public WallpaperMode WallpaperMode { get; set; } = WallpaperMode.AllowFillForceCut;
    }
}