using System;
using System.Drawing;
using System.IO;
using Extensions.netExtensions;
using Toolbox.PictureTools;

namespace Toolbox.MonitorTools
{
    public class Monitor : PictureInPicture
    {

        #region ctor

        public Monitor(Rectangle rect, string deviceName, string backgroundFile, WallpaperMode wallpaperMode)
            :base(rect)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
            {
                throw new NullReferenceException("Screen could not be initialized");
            }

            DeviceName = deviceName;
            WallpaperMode = wallpaperMode;

            TrySetFromPreviousImage(backgroundFile);
        }

        #endregion

        #region private

       
        private void TrySetFromPreviousImage(string backgroundFile)
        {
            // ReSharper disable once InvertIf
            if (!string.IsNullOrWhiteSpace(backgroundFile) && File.Exists(backgroundFile))
            {
                try
                {
                    using (var old = new Bitmap(backgroundFile))
                    {
                        if (old.Width >= (TargetArea.X + TargetArea.Width) &&
                            old.Height >= (TargetArea.Y + TargetArea.Height))
                        {
                            SetPicture(new Bitmap(old.Clone(TargetArea, old.PixelFormat)));
                            return;
                        }
                        throw new FileLoadException("Image size not compatible.");
                    }
                }
                catch //TODO Virtual screen throws exception when barely in range
                {
                    File.Delete(backgroundFile);
                }
            }
            SetPicture(new Bitmap(TargetArea.Width, TargetArea.Height));
        }


        private bool CanBeSnapped(int width, int height)
        {
            var minRatio = TargetArea.Size.ToMinRatio(PercentTopBottomCutAllowed);
            var maxRatio = TargetArea.Size.ToMaxRatio(PercentLeftRightCutAllowed);
            var imageRatio = 1.0 * width / height;
            
            return (imageRatio <= maxRatio && imageRatio >= minRatio);
        }

        /// <summary>
        /// Sets the Picture and fills the screen by cutting the Picture
        /// </summary>
        /// <param name="pictureToBeCut"></param>
        private void SetSnappedWallpaper(Image pictureToBeCut)
        {
            var targetRatio = 1.0 * TargetArea.Width / TargetArea.Height;
            var imageRatio = 1.0 * pictureToBeCut.Width / pictureToBeCut.Height;

            //shortcut, if ratio matches
            if (imageRatio.IsNearlyEqual(targetRatio, 1e-7))
            {
                SetPicture(pictureToBeCut);
            }


            Rectangle rect;
            if (targetRatio < imageRatio)
            {   // ratio to big
                var targetWidth = (int)(targetRatio * pictureToBeCut.Height);
                rect = new Rectangle(0, 0, targetWidth, pictureToBeCut.Height);
                rect.X = (pictureToBeCut.Width - rect.Width) / 2;
            }
            else
            {
                // ratio to small
                var targetHeight = (int)(pictureToBeCut.Width / targetRatio);
                rect = new Rectangle(0, 0, pictureToBeCut.Width, targetHeight);
                rect.Y = (pictureToBeCut.Height - rect.Height) / 2;
            }

            SetPicture(((Bitmap)pictureToBeCut).Clone(rect, pictureToBeCut.PixelFormat));
        }
        /// <summary>
        /// Cuts the Picture by the allowed amount and sets it as big as possible with black bars.
        /// Should be called only if it can´t be "Snapped"
        /// </summary>
        /// <param name="pictureToBeCut"></param>
        private void SetCutWallpaper(Image pictureToBeCut)
        {
            
            var targetRatio = 1.0 * TargetArea.Width / TargetArea.Height;
            var imageRatio = 1.0 * pictureToBeCut.Width / pictureToBeCut.Height;

            //shortcut, if ratio matches
            if (imageRatio.IsNearlyEqual(targetRatio, 1e-7))
            {
                SetPicture(pictureToBeCut);
            }


            Rectangle rect;
            if (targetRatio < 1.0 * pictureToBeCut.Width / pictureToBeCut.Height)
            {   // ratio to big
                var pixelsToCut = 1.0 * pictureToBeCut.Width / 100 * PercentLeftRightCutAllowed;
                rect = new Rectangle(0, 0, pictureToBeCut.Width - (int)pixelsToCut, pictureToBeCut.Height);
                rect.X = (pictureToBeCut.Width - rect.Width) / 2;
            }
            else
            {   // ratio to small
                var pixelsToCut = 1.0 * pictureToBeCut.Height / 100 * PercentTopBottomCutAllowed;
                rect = new Rectangle(0, 0, pictureToBeCut.Width, pictureToBeCut.Height - (int)pixelsToCut);
                rect.Y = (pictureToBeCut.Height - rect.Height) / 2;
            }

            SetPicture( ((Bitmap)pictureToBeCut).Clone(rect, pictureToBeCut.PixelFormat));
        }

        #endregion

        public string DeviceName { get; }
        public WallpaperMode WallpaperMode { get; set; }

        public static int PercentLeftRightCutAllowed { get; set; }
        public static int PercentTopBottomCutAllowed { get; set; }

        public void SetWallpaper(Image wall)
        {
            if (wall == null)
            {
                throw new NullReferenceException("Wallpaper can´t be null");
            }

            switch (WallpaperMode)
            {
                case WallpaperMode.AllowFill:
                    {
                        if (CanBeSnapped(wall.Width, wall.Height))
                        {
                            SetSnappedWallpaper(wall);
                        }
                        else
                        {
                            
                            SetPicture(wall);
                        }
                        break;
                    }
                case WallpaperMode.AllowFillForceCut:
                    {
                        if (CanBeSnapped(wall.Width, wall.Height))
                        {
                            SetSnappedWallpaper(wall);
                        }
                        else
                        {
                            SetCutWallpaper(wall);
                        }
                        break;
                    }
                case WallpaperMode.Fit:
                    {
                        SetPicture(wall);
                        break;
                    }
                case WallpaperMode.Fill:
                    {
                        SetSnappedWallpaper(wall);
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

    }
}
