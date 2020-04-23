using System.Collections.Generic;

namespace Contracts.Api.ResponseObjects
{
    public class WallpaperFilterResponse
    {
        /// <summary>
        /// Name of the monitor for which this result is
        /// </summary>
        public string MonitorName { get; set; }
        /// <summary>
        /// Count, how much wallpaper match given filter
        /// </summary>
        public int WallsCount { get; set; }
        /// <summary>
        /// true if wallpaper source list was sourced from cache
        /// </summary>
        public bool FromCache { get; set; }
        /// <summary>
        /// desired amount of wallpapers
        /// </summary>
        public List<WallpaperInfo> Wallpapers { get; set; } = new List<WallpaperInfo>();

    }
}
