namespace Contracts.Api.ResponseObjects
{
    public class WallpaperFilterResponse
    {
        //for command
        public string MonitorName { get; set; }
        public bool IsFavorite { get; set; }
        public bool FromCache { get; set; }
        public WallpaperInfo Wallpaper { get; set; }

        //for stats
        public int WallsCount { get; set; }
    }
}
