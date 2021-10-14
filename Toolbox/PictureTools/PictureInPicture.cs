using aemarcoCommons.Extensions.NumberExtensions;
using aemarcoCommons.Extensions.PictureExtensions;
using aemarcoCommons.Toolbox.MonitorTools;
using System;
using System.Drawing;

namespace aemarcoCommons.Toolbox.PictureTools
{
    public class PictureInPicture
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
        protected Rectangle TargetArea { get; }

        public int Width => TargetArea.Width;
        public int Height => TargetArea.Height;

        #endregion

        #region Setting the inner picture

        internal void SetWallpaper(Image wall, WallpaperMode mode, int percentTopBottomCutAllowed, int percentLeftRightCutAllowed)
        {
            bool CanBeSnapped(int width, int height)
            {
                var minRatio = TargetArea.Size.ToMinRatio(percentTopBottomCutAllowed);
                var maxRatio = TargetArea.Size.ToMaxRatio(percentLeftRightCutAllowed);
                var imageRatio = 1.0 * width / height;

                return (imageRatio <= maxRatio && imageRatio >= minRatio);
            }

            CurrentOriginal = wall ?? throw new NullReferenceException("Wallpaper can´t be null");

            switch (mode)
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
                            SetCutWallpaper(wall, percentTopBottomCutAllowed, percentLeftRightCutAllowed);
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

        public Image CurrentOriginal { get; private set; }

        private Image _currentPicture;

        /// <summary>
        /// Sets the Picture as big as possible with Black bars to keep aspect ratio
        /// </summary>
        /// <param name="image">readyToUsePicture</param>
        public void SetPicture(Image image)
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
            Graphics.FromImage(targetImg).DrawImage(image, new Rectangle(x, y, width, height));
            _currentPicture = targetImg;
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

            if (imageRatio.IsNearlyEqual(targetRatio))
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
        /// <param name="percentLeftRightCutAllowed"></param>
        /// <param name="percentTopBottomCutAllowed"></param>
        private void SetCutWallpaper(Image pictureToBeCut, int percentTopBottomCutAllowed, int percentLeftRightCutAllowed)
        {

            var targetRatio = 1.0 * TargetArea.Width / TargetArea.Height;
            var imageRatio = 1.0 * pictureToBeCut.Width / pictureToBeCut.Height;

            //shortcut, if ratio matches
            if (imageRatio.IsNearlyEqual(targetRatio))
            {
                SetPicture(pictureToBeCut);
            }


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

            SetPicture(((Bitmap)pictureToBeCut).Clone(rect, pictureToBeCut.PixelFormat));
        }

        #endregion

        #region Drawing to the outer picture

        public void DrawToGraphics(Graphics g)
        {
            g.DrawImage(_currentPicture, TargetArea);
        }

        #endregion

    }
}
