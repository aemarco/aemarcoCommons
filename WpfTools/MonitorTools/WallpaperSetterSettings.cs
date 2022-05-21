using aemarcoCommons.Toolbox.MonitorTools;
using System;
using System.IO;
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

        event EventHandler SplitSettingsChanged;
        public int HorizontalSplit { get; set; }
        public int VerticalSplit { get; set; }
        ScreenUsage ScreenUsage { get; set; }

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
        public string LockScreenFilePath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "CurrentLockScreen.jpg");


        public event EventHandler SplitSettingsChanged;
        private int _horizontalSplit = 1;
        /// <summary>
        /// Number of Slices horizontally per Monitor 
        /// </summary>
        public int HorizontalSplit
        {
            get => _horizontalSplit;
            set
            {
                if (_horizontalSplit == value)
                    return;

                _horizontalSplit = value;
                SplitSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private int _verticalSplit = 1;
        /// <summary>
        /// Number of Slices vertically per Monitor
        /// </summary>
        public int VerticalSplit
        {
            get => _verticalSplit;
            set
            {
                if (_verticalSplit == value)
                    return;

                _verticalSplit = value;
                SplitSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }



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

        /// <summary>
        /// Defines if the entire area is used, or only it´s working area
        /// </summary>
        public ScreenUsage ScreenUsage { get; set; } = ScreenUsage.All;
    }
}