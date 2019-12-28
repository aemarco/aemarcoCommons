using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Contracts.Api.RequestObjects
{
    public class WallpaperFilterReqObject
    {

        /// <summary>
        /// Response will return this name as "MonitorName"
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id of the user to resolve, used for FSK and Favorites
        /// </summary>
        public string UserId { get; set; }
        //for filter
        /// <summary>
        /// minimal desired ratio ex. 16/9 monitor is 1.77... so minimum supposed to be lower 
        /// </summary>
        public double MinRatio { get; set; }
        /// <summary>
        /// maximal desired ratio ex. 16/9 monitor is 1.77... so maximum supposed to be higher 
        /// </summary>
        public double MaxRatio { get; set; }
        /// <summary>
        /// List of "CategoryString" from ExtendedCategory (better to use id´s)
        /// </summary>
        public List<string> Categories { get; set; }
        /// <summary>
        /// List of Ids of desired Categories
        /// </summary>
        public List<int> CategoryIds { get; set; }
        /// <summary>
        /// filter down to a specific girl
        /// </summary>
        public int GirlId { get; set; }
        /// <summary>
        /// minimum Adult Level 0...100, defines lower limit
        /// </summary>
        public int MinAdult { get; set; }
        /// <summary>
        /// maximu Adult Level 0...100, defines upper limit
        /// </summary>
        public int MaxAdult { get; set; }
        /// <summary>
        /// space seperatet Search terms (slow) ex. "BMW Cabrio" will be AND
        /// </summary>
        public string Search { get; set; }
        /// <summary>
        /// picture width * height must be higher than is value
        /// </summary>
        public double MinPixels { get; set; }
        /// <summary>
        /// allowed extensions ex. { ".jpg", ".png", ".bmp", ".jpeg" }
        /// </summary>
        public List<string> Extensions { get; set; }
        /// <summary>
        /// true means that results beeing filtered to pictures marked as favorites for current user
        /// </summary>
        public bool FavoritesOnly { get; set; }

        /// <summary>
        /// defines how many results are desired for this request
        /// </summary>
        public int DesiredAmount { get; set; } = 1;



        [JsonIgnore]
        public string Signature
        {
            get
            {
                if (CategoryIds?.Any() ?? false) CategoryIds = CategoryIds.OrderBy(x => x).ToList();
                if (Categories?.Any() ?? false) Categories = Categories.OrderBy(x => x).ToList();
                if (Extensions?.Any() ?? false) Extensions = Extensions.OrderBy(x => x).ToList();
                return this.ToString();
            }
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}
