using aemarcoCommons.Extensions.FileExtensions;
using aemarcoCommons.Extensions.TextExtensions;
using aemarcoCommons.Toolbox.PictureTools;
using System;
using System.Drawing;
using System.IO;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Toolbox.MonitorTools
{
    [Obsolete]
    public class LockScreen : PictureInPicture, IWallpaperRealEstate
    {
        private readonly IWallpaperRealEstateSettings _settings;

        public LockScreen(
            Rectangle rect,
            string deviceName,
            string sourceFile,
            IWallpaperRealEstateSettings settings)
            : base(rect)
        {
            DeviceName = deviceName;
            _settings = settings;
            TargetFilePath = sourceFile;

            TrySetFromPreviousImage(TargetFilePath);
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
                            Timestamp = new DateTimeOffset(new FileInfo(sourceFile).LastWriteTime);
                            ChangedSinceDrawn = false;
                            return;
                        }
                        //if the previous file has no compatible size, then we default without exception
                    }
                }
                catch
                {
                    new FileInfo(sourceFile).TryDelete();
                }
            }
            //defaults to a black image
            Current = new Bitmap(TargetArea.Width, TargetArea.Height);
            Timestamp = DateTimeOffset.MinValue;
        }



        public string DeviceName { get; }
        public RealEstateType Type => RealEstateType.LockScreen;
        public string FriendlyName => $"Lock Screen {string.Join("-", DeviceName.GetNumbersFromText())}";
        public string TargetFilePath { get; }

        public void SetWallpaper(Image image, Color? background = null) =>
            SetWallpaper(
                image,
                _settings.WallpaperMode,
                _settings.PercentTopBottomCutAllowed,
                _settings.PercentLeftRightCutAllowed,
                background);
    }
}