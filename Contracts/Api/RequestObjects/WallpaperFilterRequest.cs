
using System;
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
        [Obsolete]
        public string UserId { get; set; }
        /// <summary>
        /// defines how many results are desired for this request,
        /// defaults to 1, max being 100
        /// </summary>
        public int DesiredAmount { get; set; } = 1;


        #endregion

        #region filter


        /// <summary>
        /// minimal desired ratio ex. 16/9 monitor is 1.77... so minimum supposed to be lower,
        /// is an alternative to DesiredWidth and DesiredHeight
        /// </summary>
        public double? MinRatio { get; set; }
        /// <summary>
        /// maximal desired ratio ex. 16/9 monitor is 1.77... so maximum supposed to be higher
        /// is an alternative to DesiredWidth and DesiredHeight
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
        /// how may percent do we allow to cut horizontally, defaults to 20, min 0, max 50
        /// only applies when DesiredWidth and DesiredHeight is used
        /// </summary>
        [Range(0, 50)]
        public int PercentLeftRightCutAllowed { get; set; } = 20;
        /// <summary>
        /// how many percent do we allow to cut vertically, defaults to 10, min 0, max 50
        /// only applies when DesiredWidth and DesiredHeight is used
        /// </summary>
        [Range(0, 50)]
        public int PercentTopBottomCutAllowed { get; set; } = 10;
        // <summary>
        /// List of Ids for desired Categories
        /// </summary>
        public List<int> CategoryIds { get; set; }
        /// <summary>
        /// minimum Adult Limit 0...100, defines lower limit
        /// </summary>
        public int MinAdult { get; set; }
        /// <summary>
        /// maximum Adult Limit 0...100, defines upper limit
        /// </summary>
        public int MaxAdult { get; set; }
        /// <summary>
        /// space separated Search terms (slow) ex. "BMW Cabrio" will be AND
        /// </summary>
        public string Search { get; set; }
        /// <summary>
        /// picture width * height must be at least this value
        /// </summary>
        public double MinPixels { get; set; }
        /// <summary>
        /// allowed extensions ex. { ".jpg", ".png", ".bmp", ".jpeg" }
        /// </summary>
        public List<string> Extensions { get; set; }

        /// <summary>
        /// filter down to a specific girl id
        /// </summary>
        public int? GirlId { get; set; }

        /// <summary>
        /// true means that results being filtered down to pictures marked as favorites for given user
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
