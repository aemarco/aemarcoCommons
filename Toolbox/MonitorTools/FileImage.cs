using aemarcoCommons.Extensions.FileExtensions;
using aemarcoCommons.Toolbox.PictureTools;
using System;
using System.Drawing;
using System.IO;

namespace aemarcoCommons.Toolbox.MonitorTools
{
    public class FileImage : PictureInPicture, IWallpaperRealEstate
    {

        private readonly FileImageSettings _settings;
        public FileImage(FileImageSettings settings)
            : base(new Rectangle(
                settings.X,
                settings.Y,
                settings.Width,
                settings.Height))
        {
            _settings = settings;
            TrySetFromPreviousImage();
        }
        private void TrySetFromPreviousImage()
        {
            // ReSharper disable once InvertIf
            if (!string.IsNullOrWhiteSpace(TargetFilePath) && File.Exists(TargetFilePath))
            {
                try
                {
                    using (var old = new Bitmap(TargetFilePath))
                    {
                        if (old.Width >= (TargetArea.X + TargetArea.Width) &&
                            old.Height >= (TargetArea.Y + TargetArea.Height))
                        {
                            Current = new Bitmap(old.Clone(TargetArea, old.PixelFormat));
                            Timestamp = new DateTimeOffset(new FileInfo(TargetFilePath).LastWriteTime);
                            ChangedSinceDrawn = false;
                            return;
                        }
                        //if the previous file has no compatible size, then we default without exception
                    }
                }
                catch
                {
                    new FileInfo(TargetFilePath).TryDelete();
                }
            }
            //defaults to a black image
            Current = new Bitmap(TargetArea.Width, TargetArea.Height);
            Timestamp = DateTimeOffset.MinValue;
        }



        public string DeviceName => _settings.DeviceName;
        public RealEstateType Type => RealEstateType.FileImage;
        public string FriendlyName => _settings.DeviceName;
        public string TargetFilePath => _settings.FilePath;


        public void SetWallpaper(Image image, Color? background = null) =>
            SetWallpaper(
                image,
                _settings.WallpaperMode,
                _settings.PercentTopBottomCutAllowed,
                _settings.PercentLeftRightCutAllowed,
                background);
    }

}