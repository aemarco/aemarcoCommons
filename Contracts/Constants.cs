using System;
using System.Collections.Generic;

namespace Contracts
{
    public static class Constants
    {
        // ReSharper disable StringLiteralTypo
        // ReSharper disable IdentifierTypo
        // ReSharper disable MemberCanBePrivate.Global

        //webgirls
        [Obsolete("Use CentralBackendSettings.WebgirlsUrl")]
        public const string WebgirlsUrl = "https://aemarco.myds.me";
       

        //wallpaper
        public const string ChangerEmail = "aeChanger@normal.com";
        public const string ChangerUserId = "52dbf093-b08c-411e-9e1d-8b31135e4707";
        public const string Watermark = "aemarco.myds.me";

        //video
        public const int TimeUntilVideoViewCoutsAsPlayedSeconds = 15;

        //thumbnails
        public const string ThumbnailSmall = "?width=300&roundedcorners=10&bgcolor=222";
        public const string ThumbnailDetail = "?width=1110&roundedcorners=40&bgcolor=222";
        public const string ThumbnailProfileSmall = "?height=450&roundedcorners=10&bgcolor=222";
        public const string ThumbnailProfileDetail = "?height=600&roundedcorners=15&bgcolor=222";
        public static IEnumerable<string> GetThumbnailQueries(bool includeProfile)
        {
            var result = new List<string>
            {
                ThumbnailSmall,
                ThumbnailDetail
            };
            if (!includeProfile) return result;
            
            result.Add(ThumbnailProfileSmall);
            result.Add(ThumbnailProfileDetail);
            return result;
        }

        // ReSharper restore StringLiteralTypo
        // ReSharper restore IdentifierTypo
        // ReSharper restore MemberCanBePrivate.Global

    }
}
