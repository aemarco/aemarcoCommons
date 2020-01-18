using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Api.ResponseObjects
{
    public class WallpaperFilterResponse
    {
        public string MonitorName { get; set; }
        public int WallsCount { get; set; }
        public bool FromCache { get; set; }
        public List<WallpaperInfo> Wallpapers { get; set; } = new List<WallpaperInfo>();

    }
}
