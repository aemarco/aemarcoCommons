using Contracts.Api.RequestObjects;
using System.Drawing;
using System.Linq;
using Extensions.netExtensions;

namespace Extensions.contentExtensions
{
    public static class ApiRequestExtensions
    {

        public static double? ToMinRatio(this WallpaperFilterRequest request)
        {
            if (request.MinRatio.HasValue) return request.MinRatio.Value;
            else if (request.DesiredWidth is int dWidth && request.DesiredHeight is int dHeight)
            {
                var rect = new Size(dWidth, dHeight);
                return rect.ToMinRatio(request.PercentTopBottomCutAllowed);
            }
            return null;
        }

        public static double? ToMaxRatio(this WallpaperFilterRequest request)
        {
            if (request.MaxRatio.HasValue) return request.MaxRatio.Value;
            else if (request.DesiredWidth is int dWidth && request.DesiredHeight is int dHeight)
            {
                var rect = new Size(dWidth, dHeight);
                return rect.ToMaxRatio(request.PercentLeftRightCutAllowed);
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
