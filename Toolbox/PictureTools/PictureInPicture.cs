using System.Drawing;
using Extensions.NumberExtensions;

namespace Toolbox.PictureTools
{
    public class PictureInPicture
    {
        protected Rectangle TargetArea { get; }
        private Image _currentPicture;

        public PictureInPicture(Rectangle targetArea)
        {
            TargetArea = targetArea;
        }


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
            if (widthRatio.IsNearlyEqual(heightRatio, 1e-7))
            {
                var img = new Bitmap(TargetArea.Width, TargetArea.Height);
                Graphics.FromImage(img).DrawImage(image, 0,0, TargetArea.Width, TargetArea.Height);
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

        public void DrawToGraphics(Graphics g)
        {
            g.DrawImage(_currentPicture, TargetArea);
        }


    }
}
