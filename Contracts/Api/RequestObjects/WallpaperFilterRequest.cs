
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Contracts.Api.RequestObjects
{


    public class WallpaperFilterRequest
    {
        #region handling

        /// <summary>
        /// Response will return this name as "MonitorName"
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id of the user to resolve, used for FSK and Favorites
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// defines how many results are desired for this request,
        /// irrelevant for Signature
        /// </summary>
        public int DesiredAmount { get; set; } = 1;


        #endregion

        #region filter


        /// <summary>
        /// minimal desired ratio ex. 16/9 monitor is 1.77... so minimum supposed to be lower 
        /// </summary>
        public double? MinRatio { get; set; }
        /// <summary>
        /// maximal desired ratio ex. 16/9 monitor is 1.77... so maximum supposed to be higher 
        /// </summary>
        public double? MaxRatio { get; set; }
        /// <summary>
        /// desired width in combination with height leads to a ready to use Querystring in result
        /// </summary>
        public int? DesiredWidth { get; set; }
        /// <summary>
        /// desired width in combination with height leads to a ready to use Querystring in result
        /// </summary>
        public int? DesiredHeight { get; set; }
        /// <summary>
        /// only applies wehn DesiredWidth and DesiredHeight is used
        /// </summary>
        [Range(0, 50)]
        public int PercentLeftRightCutAllowed { get; set; } = 20;
        /// <summary>
        /// only applies wehn DesiredWidth and DesiredHeight is used
        /// </summary>
        [Range(0, 50)]
        public int PercentTopBottomCutAllowed { get; set; } = 10;
        // <summary>
        /// List of Ids of desired Categories
        /// </summary>
        public List<int> CategoryIds { get; set; }
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
        /// filter down to a specific girl
        /// </summary>
        public int? GirlId { get; set; }

        /// <summary>
        /// true means that results beeing filtered to pictures marked as favorites for current user
        /// </summary>
        public bool FavoritesOnly { get; set; }

        #endregion


        [JsonIgnore]
        public string Signature
        {
            get
            {
                if (CategoryIds?.Any() ?? false) CategoryIds = CategoryIds.OrderBy(x => x).ToList();
                if (Extensions?.Any() ?? false) Extensions = Extensions.OrderBy(x => x).ToList();
                var current = DesiredAmount;
                DesiredAmount = 0;
                var signature = this.ToString();
                DesiredAmount = current;
                return signature;
            }
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

}
