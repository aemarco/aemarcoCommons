using System.Collections.Generic;

namespace Contracts.Messages
{
    public class FileServiceMessage
    {
        /// <summary>
        /// Specify the kind of Entity to act upon
        /// - Album (ex. Album.Import)
        /// - Video
        /// - Wallpaper
        /// </summary>
        public string EntryType { get; set; }
        /// <summary>
        /// The kind of service which is requested for a Entity, Folder or File
        /// - Import --> Imports given Folder or File
        /// </summary>
        public string ServiceType { get; set; }
        /// <summary>
        /// When provided, Entity actions will be performed in context of given user
        /// </summary>
        public string UserEmail { get; set; }
        
        /// <summary>
        /// Specify the Id of the Entity which we want to act upon
        /// ex. Thumbnail, ConvertToMp4 etc.
        /// </summary>
        public int EntryId { get; set; }
        /// <summary>
        /// Path which we like to act upon. Could be a Folder or File.
        /// ex. Folder when --> Album.Import, File when Wallpaper.Import
        /// </summary>
        public string PathToImport { get; set; }
        /// <summary>
        /// Tags to be set during an import
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// Ids of Girls which we want to reference in state Confirmed
        /// </summary>
        public List<int> GirlIds { get; set; }
        /// <summary>
        /// Adult level which we want to set on the entity
        /// </summary>
        public int AdultLevel { get; set; } = -1;
        /// <summary>
        /// Specifies a name for either the file or the album.
        /// File: defaults back to take the name from the file
        /// Album: defaults back to take the name from the folder
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Specify a category, which is being set on Wallpapers
        /// Wallpaper: Set directly on wallpaper
        /// Album: Set on all wallpapers
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// Specify a category, which is being set on Videos
        /// Video: Set directly on video,
        /// Album: Set on all videos
        /// </summary>
        public int VideoCategoryId { get; set; }
        /// <summary>
        /// Specify the ranking, which is being set on Videos
        /// Video: Set directly on video,
        /// Album: Set on all videos
        /// </summary>
        public int? VideoRanking { get; set; }

    }
}
