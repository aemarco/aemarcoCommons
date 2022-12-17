using aemarcoCommons.Extensions.FileExtensions;
using aemarcoCommons.Extensions.TextExtensions;
using aemarcoCommons.Toolbox.PictureTools;
using System.Drawing;
using System.IO;

namespace aemarcoCommons.Toolbox.MonitorTools
{
    public class LockScreen : PictureInPicture, IWallpaperRealEstate
    {
        private readonly IWallpaperRealEstateSettings _lockScreenSettings;

        public LockScreen(
            Rectangle rect,
            string deviceName,
            string sourceFile,
            IWallpaperRealEstateSettings lockScreenSettings)
            : base(rect)
        {
            DeviceName = deviceName;
            _lockScreenSettings = lockScreenSettings;

            TrySetFromPreviousImage(sourceFile);
        }

        private void TrySetFromPreviousImage(string sourceFile)
        {
            // ReSharper disable once InvertIf
            if (!string.IsNullOrWhiteSpace(sourceFile) && File.Exists(sourceFile))
            {
                try
                {
                    using (var old = new Bitmap(sourceFile))
                    {
                        if (old.Width >= (TargetArea.X + TargetArea.Width) &&
                            old.Height >= (TargetArea.Y + TargetArea.Height))
                        {
                            Current = new Bitmap(old.Clone(TargetArea, old.PixelFormat));
                            return;
                        }
                        //if the previous file is not compatible in size, then we default without exception
                    }
                }
                catch
                {
                    new FileInfo(sourceFile).TryDelete();
                }
            }
            //defaults to a black image
            Current = new Bitmap(TargetArea.Width, TargetArea.Height);
        }



        public string DeviceName { get; }
        public RealEstateType Type => RealEstateType.LockScreen;
        public string FriendlyName => $"Lock Screen {string.Join("-", DeviceName.GetNumbersFromText())}";

        public void SetWallpaper(Image wall, Color? background = null) =>
            SetWallpaper(
                wall,
                _lockScreenSettings.WallpaperMode,
                _lockScreenSettings.PercentTopBottomCutAllowed,
                _lockScreenSettings.PercentLeftRightCutAllowed,
                background);
    }
}