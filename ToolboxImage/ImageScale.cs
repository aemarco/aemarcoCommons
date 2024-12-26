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

namespace aemarcoCommons.ToolboxImage;

public static class ImageScale
{

    /// <summary>
    /// Resizes an image loaded from a stream to the specified dimensions and returns the result as a MemoryStream.
    /// </summary>
    /// <param name="source">The input stream containing the image data.</param>
    /// <param name="width">The target width for the resized image.</param>
    /// <param name="height">The target height for the resized image.</param>
    /// <param name="mode">The resizing mode to use (defaults to <see cref="ImageScaleMode.FillAndFit"/>).</param>
    /// <param name="maxVerticalCropPercentage">The maximum vertical crop percentage allowed for resizing (defaults to 10%).</param>
    /// <param name="maxHorizontalCropPercentage">The maximum horizontal crop percentage allowed for resizing (defaults to 20%).</param>
    /// <returns>A <see cref="MemoryStream"/> containing the resized image in JPEG format.</returns>
    /// <remarks>
    /// The method loads the image from the given stream, performs resizing using the specified mode, vertical, 
    /// and horizontal cropping percentages, and returns the resized image as a MemoryStream encoded as JPEG.
    /// </remarks>
    public static MemoryStream Resize(
        Stream source,
        int width,
        int height,
        ImageScaleMode mode = ImageScaleMode.FillAndFit,
        int maxHorizontalCropPercentage = 20,
        int maxVerticalCropPercentage = 10)
    {
        var image = Image.Load(source);
        image.ResizeMutate(width, height, mode, maxHorizontalCropPercentage, maxVerticalCropPercentage);

        var result = new MemoryStream();
        image.Save(result, new JpegEncoder());
        result.Seek(0, SeekOrigin.Begin);
        return result;
    }

    /// <summary>
    /// Creates a resized clone of the specified image using the given parameters.
    /// </summary>
    /// <param name="source">The image to resize and clone.</param>
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
        this Image source,
        int width,
        int height,
        ImageScaleMode mode = ImageScaleMode.FillAndFit,
        int maxHorizontalCropPercentage = 20,
        int maxVerticalCropPercentage = 10)
    {
        var result = source.Clone(_ => { });
        result.ResizeMutate(width, height, mode, maxHorizontalCropPercentage, maxVerticalCropPercentage);
        return result;
    }

    /// <summary>
    /// Resizes the current image to the specified dimensions and modifies it in-place.
    /// </summary>
    /// <param name="source">The image to resize.</param>
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
        this Image source,
        int width,
        int height,
        ImageScaleMode mode = ImageScaleMode.FillAndFit,
        int maxHorizontalCropPercentage = 20,
        int maxVerticalCropPercentage = 10)
    {
        _ = Scaling.Resize(
            source,
            width,
            height,
            mode,
            maxHorizontalCropPercentage,
            maxVerticalCropPercentage);
    }

}
