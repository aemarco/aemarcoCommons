using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace aemarcoCommons.Toolbox.PictureTools
{
    public static class PictureInPictureExtensions
    {

        public static void CreateImageFile(this IPictureInPicture content, string targetFile) =>
            new[] { content }.CreateImageFile(targetFile);

        public static void CreateImageFile(this IEnumerable<IPictureInPicture> contents, string targetFile)
        {
            using (Image image = contents.CreateOuterImage())
            {
                image.Save(targetFile, ImageFormat.Jpeg);
            }
        }


        public static Image CreateOuterImage(this IPictureInPicture realEstate) =>
            new[] { realEstate }.CreateOuterImage();


        public static Image CreateOuterImage(this IEnumerable<IPictureInPicture> realEstates)
        {
            var source = realEstates.ToList();
            var width = source.Max(x => x.TargetArea.X + x.TargetArea.Width);
            var height = source.Max(x => x.TargetArea.Y + x.TargetArea.Height);
            Image result = new Bitmap(width, height);

            source.ForEach(x => x.DrawToImage(result));

            return result;
        }

    }
}