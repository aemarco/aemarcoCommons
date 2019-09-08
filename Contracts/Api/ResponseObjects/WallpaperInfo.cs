using System;

namespace Contracts.Api.ResponseObjects
{
    public class WallpaperInfo
    {

        //Wallpaper
        public int Id { get; set; }
        public string Url { get; set; }
        public string SourceUrl { get; set; }
        public string Filename { get; set; }
        public string FileExtension { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int AdultLevel { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Tagstring { get; set; }

        //for anonym Downloader
        public Guid Auth { get; set; }


    }
}
