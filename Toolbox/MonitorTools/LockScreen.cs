using System.Drawing;
using aemarcoCommons.Toolbox.PictureTools;

namespace aemarcoCommons.Toolbox.MonitorTools
{
    public class LockScreen : PictureInPicture, IWallpaperRealEstate
    {
        private readonly IWallpaperRealEstateSettings _lockScreenSettings;

        public LockScreen(Rectangle rect, string deviceName, IWallpaperRealEstateSettings lockScreenSettings)
            :base(rect)
        {
            _lockScreenSettings = lockScreenSettings;
            DeviceName = deviceName;
        }

        public string DeviceName { get; }
        public void SetWallpaper(Image wall) =>
            SetWallpaper(
                wall, 
                _lockScreenSettings.WallpaperMode, 
                _lockScreenSettings.PercentTopBottomCutAllowed, 
                _lockScreenSettings.PercentLeftRightCutAllowed);
    }
}