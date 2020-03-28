using System;

namespace Contracts.Api.ResponseObjects
{
    public class WallpaperInfo
    {

        //Wallpaper
        /// <summary>
        /// Id in database
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// url pointing to my original file
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// delivered when using DesiredWidth and DesiredHeight
        /// attach this to the Url, and you get the already cropped picture
        /// </summary>
        public string CropQuery { get; set; }
        /// <summary>
        /// url pointing to the original source (can be null)
        /// </summary>
        public string SourceUrl { get; set; }
        /// <summary>
        /// name of the file without extension
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// extension for the File
        /// </summary>
        public string FileExtension { get; set; }
        /// <summary>
        /// Id of the Category (see Extended Category)
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// Full category path ex. "Vehicle_Cars" (see CategoryString in Extended Category)
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Adult level between 0 and 100
        /// </summary>
        public int AdultLevel { get; set; }
        /// <summary>
        /// Width of the original
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Height of the original
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// "," seperated tags (entirely free text stuff)
        /// </summary>
        public string Tagstring { get; set; }
        /// <summary>
        /// is true if this wallpaper is favorized by the user
        /// </summary>
        public bool IsFavorite { get; set; }



        public bool FromCache { get; set; }
        
    }
}
