// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Collections.Generic;

namespace Contracts.Api.ResponseObjects
{
    public class WallpaperInfo
    {

        //Wallpaper
        /// <summary>
        /// Id in database
        /// </summary>
        public int Id { get; set; } = -1;
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
        public int CategoryId { get; set; } = -1;
        /// <summary>
        /// Full category path ex. "Vehicle_Cars" (see CategoryString in Extended Category)
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Adult level between 0 and 100, 0 = noTits, 100 moreThanTits
        /// </summary>
        public int AdultLevel { get; set; } = -1;
        /// <summary>
        /// Width of the original
        /// </summary>
        public int Width { get; set; } = -1;
        /// <summary>
        /// Height of the original
        /// </summary>
        public int Height { get; set; } = -1;
        /// <summary>
        /// comma separated tags
        /// </summary>
        public string Tagstring { get; set; }
        /// <summary>
        /// List of Girl Id´s which are in the picture
        /// </summary>
        public List<int> GirlIds { get; set; } = new List<int>();
        /// <summary>
        /// is true if this wallpaper is a user favorite
        /// </summary>
        public bool IsFavorite { get; set; }
        /// <summary>
        /// is true if this wallpaper is blacklisted by the user
        /// </summary>
        public bool IsBlacklisted { get; set; }
        /// <summary>
        /// if true when sourced from cache
        /// </summary>
        public bool FromCache { get; set; }
        
    }
}
