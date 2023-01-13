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

        private Image _current;
        public Image Current
        {
            get => _current;
            protected set
            {
                if (_current == value)
                    return;

                _current = value;
                Timestamp = DateTimeOffset.Now;
                ChangedSinceDrawn = true;
            }
        }
        public DateTimeOffset Timestamp { get; protected set; }

        public bool ChangedSinceDrawn { get; protected set; }

        #endregion

        #region Setting the inner picture

        internal void SetWallpaper(
            Image wall,
            WallpaperMode mode,
            int percentTopBottomCutAllowed,
            int percentLeftRightCutAllowed,
            Color? background = null)
        {

            _wallpaperMode = mode;
            _percentTopBottomCutAllowed = percentTopBottomCutAllowed;
            _percentLeftRightCutAllowed = percentLeftRightCutAllowed;
            _background = background;

            Current = wall;
        }

        #endregion

        #region Drawing to the outer picture

        public void DrawToImage(Image image)
        {
            using (var currentContent = GetCurrentContent())
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.DrawImage(currentContent, TargetArea);
                }
            }
            ChangedSinceDrawn = false;
        }


        private WallpaperMode _wallpaperMode;
        private int _percentTopBottomCutAllowed;
        private int _percentLeftRightCutAllowed;
        private Color? _background;
        private Image GetCurrentContent()
        {
            var minRatio = TargetArea.Size.ToMinRatio(_percentTopBottomCutAllowed);
            var maxRatio = TargetArea.Size.ToMaxRatio(_percentLeftRightCutAllowed);
            var imageRatio = 1.0 * Current.Width / Current.Height;
            var canBeSnapped = imageRatio <= maxRatio && imageRatio >= minRatio;

            switch (_wallpaperMode)
            {
                case WallpaperMode.AllowFill:
                    {
                        return canBeSnapped
                            ? GetSnapped()
                            : GetPicture(Current);
                    }
                case WallpaperMode.AllowFillForceCut:
                    {
                        return canBeSnapped
                            ? GetSnapped()
                            : GetCutWallpaper();
                    }
                case WallpaperMode.Fit:
                    {
                        return GetPicture(Current);
                    }
                case WallpaperMode.Fill:
                    {
                        return GetSnapped();
                    }
                default:
                    throw new NotSupportedException("WallpaperMode not supported");
            }
        }


        //Uniform
        /// <summary>
        /// Sets the Picture as big as possible with Black bars to keep aspect ratio
        /// </summary>
        /// <param name="image">readyToUsePicture</param>
        /// <param name="background">color of the background</param>
        private Image GetPicture(Image image)
        {
            //shortcut if the picture already fits
            if (image.Width == TargetArea.Width && image.Height == TargetArea.Height)
            {
                return new Bitmap(image);
            }

            var heightRatio = TargetArea.Height / (double)image.Height;
            var widthRatio = TargetArea.Width / (double)image.Width;


            //shortcut if the ratio matches and only resize is needed 
            if (widthRatio.IsNearlyEqual(heightRatio))
            {
                var img = new Bitmap(TargetArea.Width, TargetArea.Height);
                Graphics.FromImage(img).DrawImage(image, 0, 0, TargetArea.Width, TargetArea.Height);
                return img;
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

            g.Clear(_background ?? Color.Black);
            g.DrawImage(image, new Rectangle(x, y, width, height));

            return targetImg;
        }

        //UniformToFill
        /// <summary>
        /// Sets the Picture and fills the screen by cutting the Picture
        /// </summary>
        /// <param name="pictureToBeCut"></param>
        /// <param name="background">color of the background</param>
        private Image GetSnapped()
        {
            var targetRatio = 1.0 * TargetArea.Width / TargetArea.Height;
            var imageRatio = 1.0 * Current.Width / Current.Height;


            //shortcut, if ratio matches
            if (imageRatio.IsNearlyEqual(targetRatio))
            {
                return GetPicture(Current);
            }

            Rectangle rect;
            if (targetRatio < imageRatio)
            {   // ratio to big
                var targetWidth = (int)(targetRatio * Current.Height);
                rect = new Rectangle(0, 0, targetWidth, Current.Height);
                rect.X = (Current.Width - rect.Width) / 2;
            }
            else
            {
                // ratio to small
                var targetHeight = (int)(Current.Width / targetRatio);
                rect = new Rectangle(0, 0, Current.Width, targetHeight);
                rect.Y = (Current.Height - rect.Height) / 2;
            }

            return GetPicture(((Bitmap)Current).Clone(rect, Current.PixelFormat));
        }

        //LimitedUniformToFill
        /// <summary>
        /// Cuts the Picture by the allowed amount and sets it as big as possible with black bars.
        /// Should be called only if it can´t be "Snapped"
        /// </summary>
        /// <param name="pictureToBeCut"></param>
        /// <param name="percentLeftRightCutAllowed"></param>
        /// <param name="percentTopBottomCutAllowed"></param>
        /// <param name="background">color of the background</param>
        private Image GetCutWallpaper()
        {

            var targetRatio = 1.0 * TargetArea.Width / TargetArea.Height;
            var imageRatio = 1.0 * Current.Width / Current.Height;

            //shortcut, if ratio matches
            if (imageRatio.IsNearlyEqual(targetRatio))
            {
                return GetPicture(Current);
            }

            Rectangle rect;
            if (targetRatio < 1.0 * Current.Width / Current.Height)
            {   // ratio to big
                var pixelsToCut = 1.0 * Current.Width / 100 * _percentLeftRightCutAllowed;
                rect = new Rectangle(0, 0, Current.Width - (int)pixelsToCut, Current.Height);
                rect.X = (Current.Width - rect.Width) / 2;
            }
            else
            {   // ratio to small
                var pixelsToCut = 1.0 * Current.Height / 100 * _percentTopBottomCutAllowed;
                rect = new Rectangle(0, 0, Current.Width, Current.Height - (int)pixelsToCut);
                rect.Y = (Current.Height - rect.Height) / 2;
            }

            return GetPicture(((Bitmap)Current).Clone(rect, Current.PixelFormat));
        }

        #endregion

    }
}
