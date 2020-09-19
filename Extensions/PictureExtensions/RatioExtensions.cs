using System;
using System.Drawing;

namespace aemarcoCommons.Extensions.PictureExtensions
{
    public static class RatioExtensions
    {
        /// <summary>
        /// minRatio means that the picture is currently not wide enough, hence needs cutting at top and bottom.
        /// minRatio results in a ratio Calculation with a imaginary picture, which happens to have the same width,
        /// but given height is considered already cut, therefor it is a "in hundred calculation" with given percentage.
        /// </summary>
        /// <param name="targetSize">width and height</param>
        /// <param name="percentTopBottomCutAllowed">total allowed percentage to be cut at top and bottom</param>
        /// <returns>minimum ratio</returns>
        public static double ToMinRatio(this Size targetSize, int percentTopBottomCutAllowed)
        {
            if (targetSize.Width <= 0 || targetSize.Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(targetSize));

            if (percentTopBottomCutAllowed > 100 || percentTopBottomCutAllowed < 0) 
                throw new ArgumentOutOfRangeException(nameof(percentTopBottomCutAllowed));

            var maxHeight = 100.0 * targetSize.Height / (100 - percentTopBottomCutAllowed);
            return Math.Round(targetSize.Width / maxHeight, 2);
        }

        public static double ToMaxRatio(this Size targetSize, int percentLeftRightCutAllowed)
        {
            if (targetSize.Width <= 0 || targetSize.Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(targetSize));

            if (percentLeftRightCutAllowed > 100 || percentLeftRightCutAllowed < 0)
                throw new ArgumentOutOfRangeException(nameof(percentLeftRightCutAllowed));


            var maxWidth = 100.0 * targetSize.Width / (100 - percentLeftRightCutAllowed);
            var maxRatio = maxWidth / targetSize.Height;
            
            if (double.IsPositiveInfinity(maxRatio)) maxRatio = double.MaxValue;
            
            return Math.Round(maxRatio, 2);
        }

        public static double ToMinRatio(this Rectangle rectangle, int percentTopBottomCutAllowed)
        {
            return new Size(rectangle.Width, rectangle.Height).ToMinRatio(percentTopBottomCutAllowed);
        }

        public static double ToMaxRatio(this Rectangle rectangle, int percentLeftRightCutAllowed)
        {
            return new Size(rectangle.Width, rectangle.Height).ToMaxRatio(percentLeftRightCutAllowed);
        }



    }
}
