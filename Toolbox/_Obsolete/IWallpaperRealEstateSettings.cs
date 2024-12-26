using System;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Toolbox.MonitorTools
{

    [Obsolete]
    public interface IWallpaperRealEstateSettings
    {
        WallpaperMode WallpaperMode { get; }
        int PercentTopBottomCutAllowed { get; set; }
        int PercentLeftRightCutAllowed { get; set; }
    }

}