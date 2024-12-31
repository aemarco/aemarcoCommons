/*
 * Copyright 2024 aemarco
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace aemarcoCommons.ToolboxImage.Extensions;

public static class ImageExtensions
{

    /// <summary>
    /// Creates a resized clone of the specified image using the given parameters.
    /// </summary>
    /// <param name="image">The image to resize and clone.</param>
    /// <param name="width">The target width for the resized image.</param>
    /// <param name="height">The target height for the resized image.</param>
    /// <param name="mode">The resizing mode to use (defaults to <see cref="ImageScaleMode.FillAndFit"/>).</param>
    /// <param name="maxVerticalCropPercentage">The maximum vertical crop percentage allowed for resizing (defaults to 10%).</param>
    /// <param name="maxHorizontalCropPercentage">The maximum horizontal crop percentage allowed for resizing (defaults to 20%).</param>
    /// <returns>A new instance of <see cref="Image"/> that is a resized version of the original image.</returns>
    /// <remarks>
    /// This method will not modify the original image. Instead, it will clone the source image, resize it according 
    /// to the specified parameters, and return the modified clone. The original image remains unmodified.
    /// </remarks>
    public static Image ResizeClone(
        this Image image,
        int width,
        int height,
        ImageScaleMode mode = ImageScaleMode.FillAndFit,
        int maxHorizontalCropPercentage = 20,
        int maxVerticalCropPercentage = 10)
    {
        var result = image.Clone(_ => { });
        result.ResizeMutate(width, height, mode, maxHorizontalCropPercentage, maxVerticalCropPercentage);
        return result;
    }


    /// <summary>
    /// Resizes the current image to the specified dimensions and modifies it in-place.
    /// </summary>
    /// <param name="image">The image to resize.</param>
    /// <param name="width">The target width for the resized image.</param>
    /// <param name="height">The target height for the resized image.</param>
    /// <param name="mode">The resizing mode to use (defaults to <see cref="ImageScaleMode.FillAndFit"/>).</param>
    /// <param name="maxVerticalCropPercentage">The maximum vertical crop percentage allowed for resizing (defaults to 10%).</param>
    /// <param name="maxHorizontalCropPercentage">The maximum horizontal crop percentage allowed for resizing (defaults to 20%).</param>
    /// <remarks>
    /// This method modifies the original image directly by resizing it according to the specified dimensions 
    /// and crop settings. The resized image will reflect the given mode (FillAndFit, Fill, or Fit) and crop limits.
    /// </remarks>
    public static void ResizeMutate(
        this Image image,
        int width,
        int height,
        ImageScaleMode mode = ImageScaleMode.FillAndFit,
        int maxHorizontalCropPercentage = 20,
        int maxVerticalCropPercentage = 10)
    {

        // Aspect ratios
        var imageRatio = 1.0 * image.Width / image.Height;
        var targetRatio = 1.0 * width / height;

        // Determine if cropping is allowed based on the ratio constraints
        var size = new System.Drawing.Size(width, height);
        var minRatio = size.ToMinRatio(maxVerticalCropPercentage);
        var maxRatio = size.ToMaxRatio(maxHorizontalCropPercentage);

        var cropAllowed = minRatio <= imageRatio && imageRatio <= maxRatio;
        var toWide = imageRatio > targetRatio;
        var cutWidth = (int)(maxHorizontalCropPercentage / 100.0 * image.Width);
        var cutHeight = (int)(maxVerticalCropPercentage / 100.0 * image.Height);
        _ = (mode, cropAllowed, toWide) switch
        {
            // Resize the image while preserving the aspect ratio, but crop to fit the target area.
            (ImageScaleMode.FillAndFit, true, _) => image
                .MutateResize(width, height, ResizeMode.Crop),

            // Resize the image while preserving the aspect ratio, but crop horizontal to fit the target area.
            (ImageScaleMode.FillAndFit, false, true) => image
                .MutateCrop(cutWidth / 2, 0, image.Width - cutWidth, image.Height)
                .MutateResize(width, height, ResizeMode.Pad),

            // Resize the image while preserving the aspect ratio, but crop vertical to fit the target area.
            (ImageScaleMode.FillAndFit, false, false) => image
                .MutateCrop(0, cutHeight / 2, image.Width, image.Height - cutHeight)
                .MutateResize(width, height, ResizeMode.Pad),

            // Resize the image while preserving the aspect ratio, but crop to fit the target area.
            (ImageScaleMode.Fill, _, _) => image
                .MutateResize(width, height, ResizeMode.Crop),

            // Resize the image while preserving the aspect ratio, and padding as much as necessary
            (ImageScaleMode.Fit, _, _) => image
                .MutateResize(width, height, ResizeMode.Pad),
            _ => throw new NotSupportedException("Nope")
        };
    }


    private static Image MutateCrop(this Image image, int x, int y, int width, int height)
    {
        image.Mutate(o => o
            .Crop(new Rectangle(x, y, width, height))
        );
        return image;
    }
    private static Image MutateResize(this Image image, int width, int height, ResizeMode mode)
    {
        image.Mutate(x => x
            .Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = mode
            })
        );
        return image;
    }
}
