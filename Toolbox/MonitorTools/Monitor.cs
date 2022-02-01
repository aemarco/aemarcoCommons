using aemarcoCommons.Toolbox.PictureTools;
using Autofac;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.IO;

namespace aemarcoCommons.Toolbox.MonitorTools
{
    public class Monitor : PictureInPicture, IWallpaperRealEstate
    {

        #region ctor

        private readonly IWallpaperRealEstateSettings _monitorSettings;
        public Monitor(Rectangle rect, string deviceName, string sourceFile, IWallpaperRealEstateSettings monitorSettings)
            : base(rect)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
                throw new NullReferenceException("Screen could not be initialized");


            DeviceName = deviceName;
            _monitorSettings = monitorSettings;

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
                            SetPicture(new Bitmap(old.Clone(TargetArea, old.PixelFormat)));
                            return;
                        }
                        //if the previous file is not compatible in size, then we default without exception
                    }
                }
                catch (Exception ex)
                {
                    File.Delete(sourceFile);

                    if (Bootstrapper.RootScope != null && 
                        Bootstrapper.RootScope.TryResolve(out ILogger<Monitor> logger))
                    {
                        logger.LogError(ex, "Could not reinitialize picture for monitor with Target area {targetArea}", TargetArea);
                    }
                }
            }
            //defaults to a black image
            SetPicture(new Bitmap(TargetArea.Width, TargetArea.Height));
        }



        #endregion

        public string DeviceName { get; }

        public void SetWallpaper(Image wall) =>
            SetWallpaper(
                wall,
                _monitorSettings.WallpaperMode,
                _monitorSettings.PercentTopBottomCutAllowed,
                _monitorSettings.PercentLeftRightCutAllowed);

    }
}
