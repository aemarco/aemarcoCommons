using Contracts.Api.RequestObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions.contentExtensions
{
    public static class ApiRequestExtensions
    {

        public static double? ToMinRatio(this WallpaperFilterRequest request)
        {
            return request.MinRatio ?? request.ToMinRatioFromDimensions();

        }

        public static double? ToMaxRatio(this WallpaperFilterRequest request)
        {
            return request.MaxRatio ?? request.ToMaxRatioFromDimensions();

        }

        public static double? ToMinRatioFromDimensions(this WallpaperFilterRequest request)
        {
            if (request.DesiredWidth is int dWidth && request.DesiredHeight is int dHeight)
            {
                double maxHeight = 100.0 * dHeight / (100 - request.PercentTopBottomCutAllowed);
                return Math.Round(dWidth / maxHeight, 2);
            }
            return null;
        }

        public static double? ToMaxRatioFromDimensions(this WallpaperFilterRequest request)
        {
            if (request.DesiredWidth is int dWidth && request.DesiredHeight is int dHeight)
            {

                double maxWidth = 100.0 * dWidth / (100 - request.PercentLeftRightCutAllowed);
                var maxratio = maxWidth / dHeight;
                if (maxratio == double.PositiveInfinity) maxratio = double.MaxValue;
                return Math.Round(maxratio, 2);
            }
            return null;
        }


        public static bool ToUserIsSupervisorInfo(this WallpaperFilterRequest request, bool isSupervisor)
        {
            var cheat = CheatCode.Cheats.FirstOrDefault(x => request.Search?.Contains(x.Key) ?? false);
            if (cheat != null)
            {
                isSupervisor |= cheat.IsSupervisor;
            }
            return isSupervisor;
        }

        public static int ToUserMaxAdultLevel(this WallpaperFilterRequest request, int usersMax)
        {
            var cheat = CheatCode.Cheats.FirstOrDefault(x => request.Search?.Contains(x.Key) ?? false);
            if (cheat != null)
            {
                usersMax = cheat.MaxAdult;

            }
            return usersMax;
        }

        public static string ToCheatFreeSearch(this WallpaperFilterRequest request)
        {
            string result = request.Search;
            if (!string.IsNullOrWhiteSpace(result))
            {
                foreach (var cheat in CheatCode.Cheats)
                {
                    result = result.Replace(cheat.Key, string.Empty);
                }
            }
            return result;
        }





    }
}
