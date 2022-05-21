using aemarcoCommons.Toolbox.PictureTools;
using System.Drawing;

namespace aemarcoCommons.Toolbox.MonitorTools
{
    public class LockScreen : PictureInPicture, IWallpaperRealEstate
    {
        private readonly IWallpaperRealEstateSettings _lockScreenSettings;

        public LockScreen(
            Rectangle rect,
            IWallpaperRealEstateSettings lockScreenSettings)
            : base(rect)
        {
            _lockScreenSettings = lockScreenSettings;

        }

        public string DeviceName => nameof(LockScreen);
        public RealEstateType Type => RealEstateType.LockScreen;

        public void SetWallpaper(Image wall, Color? background = null) =>
            SetWallpaper(
                wall,
                _lockScreenSettings.WallpaperMode,
                _lockScreenSettings.PercentTopBottomCutAllowed,
                _lockScreenSettings.PercentLeftRightCutAllowed,
                background);
    }
}