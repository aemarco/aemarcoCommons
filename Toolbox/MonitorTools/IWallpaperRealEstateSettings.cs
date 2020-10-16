namespace aemarcoCommons.Toolbox.MonitorTools
{
    public interface IWallpaperRealEstateSettings
    {
        WallpaperMode WallpaperMode { get; }
        int PercentTopBottomCutAllowed { get; }
        int PercentLeftRightCutAllowed { get; }
    }

}