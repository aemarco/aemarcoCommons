using aemarcoCommons.Toolbox.MonitorTools;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace aemarcoCommons.Toolbox.PictureTools
{
    public static class PictureResize
    {


        public static MemoryStream ResizePicture(
            Stream stream,
            int targetWidth,
            int targetHeight,
            WallpaperMode mode = WallpaperMode.AllowFillForceCut,
            int? percentTopBottomCutAllowed = null,
            int? percentLeftRightCutAllowed = null,
            ImageFormat format = null,
            Color? background = null)
        {
            var resultImage = ResizePicture(
                Image.FromStream(stream),
                targetWidth, targetHeight,
                mode, percentTopBottomCutAllowed, percentLeftRightCutAllowed,
                background);

            var ms = new MemoryStream();
            resultImage.Save(ms, format ?? ImageFormat.Jpeg);
            return ms;
        }

        public static Image ResizePicture(
            Image sourceImage,
            int targetWidth,
            int targetHeight,
            WallpaperMode mode = WallpaperMode.AllowFillForceCut,
            int? percentTopBottomCutAllowed = null,
            int? percentLeftRightCutAllowed = null,
            Color? background = null)
        {
            var pip = new PictureInPicture(new Rectangle(0, 0, targetWidth, targetHeight));
            pip.SetWallpaper(
                sourceImage,
                mode,
                percentTopBottomCutAllowed ?? 11,
                percentLeftRightCutAllowed ?? 25);

            var image = new Bitmap(targetWidth, targetHeight);
            var graphic = Graphics.FromImage(image);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            graphic.Clear(background ?? Color.Black);
            pip.DrawToGraphics(graphic);

            return image;
        }










    }
}
