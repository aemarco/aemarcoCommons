using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Api.RequestObjects
{
    public class WallpaperCrawlResult
    {
        public string Name { get; set; }
        public List<WallpaperCrawlAlbumEntry> AlbumEntries  { get; set; }
        public List<WallpaperCrawlWallEntry> WallEntries  { get; set; }

        public TimeSpan? DelayTillNextDue { get; set; }
    }

    public class WallpaperCrawlAlbumEntry
    {
        public string Name { get; set; }
        public List<WallpaperCrawlWallEntry> Entries { get; set; }

    }
    public class WallpaperCrawlWallEntry
    {
        public string Url { get; set; }
        public string ThumbnailUrl  { get; set; }
        public string FileName  { get; set; }
        public string Extension  { get; set; }
        public string Category  { get; set; }
        public string SiteCategory  { get; set; }
        public List<string> Tags  { get; set; }
        public int SuggestedMinAdultLevel { get; set; }
        public int SuggestedMaxAdultLevel { get; set; }

        public string FileContentAsBase64String { get; set; }

    }

}
