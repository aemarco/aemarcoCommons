using aemarcoCommons.Extensions.NumberExtensions;
using aemarcoCommons.Extensions.PictureExtensions;
using aemarcoCommons.Toolbox.MonitorTools;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace aemarcoCommons.Toolbox.PictureTools
{
    public class PictureInPicture : IPictureInPicture
    {
        #region ctor

        /// <summary>
        /// Allows to set a picture, and draw it to a Graphics object
        /// </summary>
        /// <param name="targetArea">x, y coordinates and width, height which the picture should be placed</param>
        public PictureInPicture(Rectangle targetArea)
        {
            TargetArea = targetArea;
        }

        public Rectangle TargetArea { get; }
        public DateTimeOffset Timestamp { get; private set; }
        public Image CurrentOriginal { get; private set; }

        #endregion

        #region Setting the inner picture

        internal void SetWallpaper(
            Image wall,
            WallpaperMode mode,
            int percentTopBottomCutAllowed,
            int percentLeftRightCutAllowed,
            Color? background = null)
        {
            bool CanBeSnapped(int width, int height)
            {
                var minRatio = TargetArea.Size.ToMinRatio(percentTopBottomCutAllowed);
                var maxRatio = TargetArea.Size.ToMaxRatio(percentLeftRightCutAllowed);
                var imageRatio = 1.0 * width / height;

                return (imageRatio <= maxRatio && imageRatio >= minRatio);
            }

            switch (mode)
            {
                case WallpaperMode.AllowFill:
                    {
                        if (CanBeSnapped(wall.Width, wall.Height))
                            SetSnapped(wall, background);
                        else
                            SetPicture(wall, background);

                        break;
                    }
                case WallpaperMode.AllowFillForceCut:
                    {
                        if (CanBeSnapped(wall.Width, wall.Height))
                            SetSnapped(wall, background);
                        else
                            SetCutWallpaper(wall, percentTopBottomCutAllowed, percentLeftRightCutAllowed, background);
                        break;
                    }
                case WallpaperMode.Fit:
                    {
                        SetPicture(wall, background);
                        break;
                    }
                case WallpaperMode.Fill:
                    {
                        SetSnapped(wall, background);
                        break;
                    }
                default:
                    throw new NotSupportedException("WallpaperMode not supported");
            }
        }

        private Image _currentPicture;

        /// <summary>
        /// Sets the Picture as big as possible with Black bars to keep aspect ratio
        /// </summary>
        /// <param name="image">readyToUsePicture</param>
        /// <param name="background">color of the background</param>
        protected void SetPicture(Image image, Color? background = null)
        {
            //shortcut if the picture already fits
            if (image.Width == TargetArea.Width && image.Height == TargetArea.Height)
            {
                _currentPicture = new Bitmap(image);
                return;
            }

            var heightRatio = TargetArea.Height / (double)image.Height;
            var widthRatio = TargetArea.Width / (double)image.Width;


            //shortcut if the ratio matches and only resize is needed 
            if (widthRatio.IsNearlyEqual(heightRatio))
            {
                var img = new Bitmap(TargetArea.Width, TargetArea.Height);
                Graphics.FromImage(img).DrawImage(image, 0, 0, TargetArea.Width, TargetArea.Height);
                _currentPicture = img;
                return;
            }

            //black bars needed :(
            int height, width;
            var x = 0;
            var y = 0;
            if (heightRatio < widthRatio) //picture to narrow for monitor
            {
                width = (int)(image.Width * heightRatio);
                height = (int)(image.Height * heightRatio);
                x = (int)((TargetArea.Width - width) / 2f);
            }
            else //picture to wide for monitor
            {
                width = (int)(image.Width * widthRatio);
                height = (int)(image.Height * widthRatio);
                y = (int)((TargetArea.Height - height) / 2f);
            }
            var targetImg = new Bitmap(TargetArea.Width, TargetArea.Height);
            var g = Graphics.FromImage(targetImg);

            g.Clear(background ?? Color.Black);
            g.DrawImage(image, new Rectangle(x, y, width, height));
            _currentPicture = targetImg;


            Timestamp = DateTimeOffset.Now;
            CurrentOriginal = image;
        }

        /// <summary>
        /// Sets the Picture and fills the screen by cutting the Picture
        /// </summary>
        /// <param name="pictureToBeCut"></param>
        /// <param name="background">color of the background</param>
        private void SetSnapped(Image pictureToBeCut, Color? background = null)
        {
            var targetRatio = 1.0 * TargetArea.Width / TargetArea.Height;
            var imageRatio = 1.0 * pictureToBeCut.Width / pictureToBeCut.Height;

            //shortcut, if ratio matches

            if (imageRatio.IsNearlyEqual(targetRatio))
            {
                SetPicture(pictureToBeCut, background);
            }
            else
            {
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

                SetPicture(((Bitmap)pictureToBeCut).Clone(rect, pictureToBeCut.PixelFormat), background);
            }

            Timestamp = DateTimeOffset.Now;
            CurrentOriginal = pictureToBeCut;
        }

        /// <summary>
        /// Cuts the Picture by the allowed amount and sets it as big as possible with black bars.
        /// Should be called only if it can´t be "Snapped"
        /// </summary>
        /// <param name="pictureToBeCut"></param>
        /// <param name="percentLeftRightCutAllowed"></param>
        /// <param name="percentTopBottomCutAllowed"></param>
        /// <param name="background">color of the background</param>
        private void SetCutWallpaper(
            Image pictureToBeCut,
            int percentTopBottomCutAllowed,
            int percentLeftRightCutAllowed,
            Color? background = null)
        {

            var targetRatio = 1.0 * TargetArea.Width / TargetArea.Height;
            var imageRatio = 1.0 * pictureToBeCut.Width / pictureToBeCut.Height;

            //shortcut, if ratio matches
            if (imageRatio.IsNearlyEqual(targetRatio))
            {
                SetPicture(pictureToBeCut, background);
            }
            else
            {
                Rectangle rect;
                if (targetRatio < 1.0 * pictureToBeCut.Width / pictureToBeCut.Height)
                {   // ratio to big
                    var pixelsToCut = 1.0 * pictureToBeCut.Width / 100 * percentLeftRightCutAllowed;
                    rect = new Rectangle(0, 0, pictureToBeCut.Width - (int)pixelsToCut, pictureToBeCut.Height);
                    rect.X = (pictureToBeCut.Width - rect.Width) / 2;
                }
                else
                {   // ratio to small
                    var pixelsToCut = 1.0 * pictureToBeCut.Height / 100 * percentTopBottomCutAllowed;
                    rect = new Rectangle(0, 0, pictureToBeCut.Width, pictureToBeCut.Height - (int)pixelsToCut);
                    rect.Y = (pictureToBeCut.Height - rect.Height) / 2;
                }

                SetPicture(((Bitmap)pictureToBeCut).Clone(rect, pictureToBeCut.PixelFormat), background);
            }

            Timestamp = DateTimeOffset.Now;
            CurrentOriginal = pictureToBeCut;
        }

        #endregion

        #region Drawing to the outer picture

        public void DrawToImage(Image image)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(_currentPicture, TargetArea);
            }
        }

        #endregion

    }
}
