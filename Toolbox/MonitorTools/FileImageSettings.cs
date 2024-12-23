namespace aemarcoCommons.Toolbox.MonitorTools
{
    public class FileImageSettings : IWallpaperRealEstateSettings
    {
        public string DeviceName { get; set; }
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Width { get; set; } = 1920;
        public int Height { get; set; } = 1080;
        public string FilePath { get; set; }

        public WallpaperMode WallpaperMode { get; set; } = WallpaperMode.AllowFillForceCut;
        public int PercentTopBottomCutAllowed { get; set; } = 20;
        public int PercentLeftRightCutAllowed { get; set; } = 10;
    }
}