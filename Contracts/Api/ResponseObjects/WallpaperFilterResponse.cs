namespace Contracts.Api.ResponseObjects
{
    public class WallpaperFilterResponse
    {
        //for command
        /// <summary>
        /// name from corresponding filter
        /// </summary>
        public string MonitorName { get; set; }
        /// <summary>
        /// true if this picture is marked as favorite by current user
        /// </summary>
        public bool IsFavorite { get; set; }
        /// <summary>
        /// true if filtered list was beeing used from cache
        /// </summary>
        public bool FromCache { get; set; }
        /// <summary>
        /// desired random Wallpaper (Wallpapers marked as favorites will be more likely to come)
        /// </summary>
        public WallpaperInfo Wallpaper { get; set; }

        //for stats
        /// <summary>
        /// number of Wallpapers matching current filter
        /// </summary>
        public int WallsCount { get; set; }
    }
}
