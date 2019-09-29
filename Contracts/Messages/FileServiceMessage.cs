using System.Collections.Generic;

namespace Contracts.Messages
{
    public class FileServiceMessage
    {
        public string EntryType { get; set; }
        public string ServiceType { get; set; }

        //used for Thumbnail and ConvertToMp4
        public int EntryId { get; set; }

        //used for Import
        public string PathToImport { get; set; }
        public List<string> Tags { get; set; }
        public List<int> GirlIds { get; set; }

        //adds for Wallpaper
        public int CategoryId { get; set; }

    }
}
