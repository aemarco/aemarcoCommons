﻿using aemarcoCommons.Extensions.FileExtensions;
using aemarcoCommons.Toolbox.PictureTools;
using System;
using System.Drawing;
using System.IO;

namespace aemarcoCommons.Toolbox.MonitorTools
{
    public class Monitor : PictureInPicture, IWallpaperRealEstate
    {

        #region ctor

        private readonly IWallpaperRealEstateSettings _monitorSettings;
        public Monitor(
            Rectangle rect,
            string deviceName,
            string sourceFile,
            IWallpaperRealEstateSettings monitorSettings,
            RealEstateType realEstateType)
            : base(rect)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
                throw new NullReferenceException("Screen could not be initialized");


            DeviceName = deviceName;
            Type = realEstateType;
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
                catch
                {
                    new FileInfo(sourceFile).TryDelete();
                }
            }
            //defaults to a black image
            SetPicture(new Bitmap(TargetArea.Width, TargetArea.Height));
        }

        #endregion

        public string DeviceName { get; }

        public RealEstateType Type { get; }

        public void SetWallpaper(Image wall, Color? background = null) =>
            SetWallpaper(
                wall,
                _monitorSettings.WallpaperMode,
                _monitorSettings.PercentTopBottomCutAllowed,
                _monitorSettings.PercentLeftRightCutAllowed,
                background);

    }
}
