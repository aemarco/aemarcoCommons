using System.Collections.Generic;

namespace Contracts.Api.RequestObjects
{
    public class WallpaperFilterReqObject
    {
        //for requestor        
        public string Name { get; set; }

        //for all
        public string UserId { get; set; }

        //for filter
        public double MinRatio { get; set; }
        public double MaxRatio { get; set; }
        public List<string> Categories { get; set; }
        public List<int> CategoryIds { get; set; }
        public int MinAdult { get; set; }
        public int MaxAdult { get; set; }
        public string Search { get; set; }
        public double MinPixels { get; set; }
        public List<string> Extensions { get; set; }
        public bool FavoritesOnly { get; set; }
    }
}
