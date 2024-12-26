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

using aemarcoCommons.Extensions.PictureExtensions;

namespace aemarcoCommons.ToolboxImage.Processing;

internal static class Scaling
{

    /// <summary>
    /// Resizes the given image based on the specified scale mode and other parameters.
    /// </summary>
    /// <param name="image">The image to resize.</param>
    /// <param name="width">The target width.</param>
    /// <param name="height">The target height.</param>
    /// <param name="mode">The scale mode.</param>
    /// <param name="maxVerticalCropPercentage">Percentage of height that can be cut for resizing.</param>
    /// <param name="maxHorizontalCropPercentage">Percentage of width that can be cut for resizing.</param>
    /// <returns>The resized image.</returns>
    public static Image Resize(
        Image image,
        int width,
        int height,
        ImageScaleMode mode,
        int maxHorizontalCropPercentage,
        int maxVerticalCropPercentage)

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
        var cutWidth = (int)((maxHorizontalCropPercentage / 100.0) * image.Width);
        var cutHeight = (int)((maxVerticalCropPercentage / 100.0) * image.Height);
        return (mode, cropAllowed, toWide) switch
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
