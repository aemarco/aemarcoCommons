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

using SixLabors.ImageSharp.Metadata.Profiles.Exif;

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
        image.Save(result, new JpegEncoder { Quality = 75 });
        result.Seek(0, SeekOrigin.Begin);
        return result;
    }

    /// <summary>
    /// Scales an image loaded from a stream down until it has at most the given number of pixels and returns the result as a MemoryStream.
    /// </summary>
    /// <param name="source">The input stream containing the image data.</param>
    /// <param name="maxPixels">The maximum number of pixels (width × height) of the result.</param>
    /// <returns>A <see cref="MemoryStream"/> containing the image in JPEG format.</returns>
    /// <remarks>
    /// The aspect ratio is kept, nothing is cropped or padded, and an image that already has at most
    /// <paramref name="maxPixels"/> pixels is not enlarged. Unlike a box, a pixel budget fits every aspect ratio.
    /// A JPEG that needs no resize and carries no EXIF rotation is returned byte for byte, so it is not compressed a second time;
    /// everything else is rotated upright and written as a new JPEG with quality 95.
    /// </remarks>
    public static MemoryStream ShrinkToPixels(Stream source, long maxPixels)
    {
        var info = Image.Identify(source);
        source.Seek(0, SeekOrigin.Begin);
        var pixels = (long)info.Width * info.Height;
        var isRotated =
            info.Metadata.ExifProfile is { } exif &&
            exif.TryGetValue(ExifTag.Orientation, out var orientation) &&
            orientation.Value > 1;
        if (pixels <= maxPixels &&
            !isRotated &&
            info.Metadata.DecodedImageFormat is JpegFormat)
        {
            var copy = new MemoryStream();
            source.CopyTo(copy);
            copy.Seek(0, SeekOrigin.Begin);
            return copy;
        }

        using var image = Image.Load(source);
        image.Mutate(x => x.AutoOrient());
        var orientedPixels = (long)image.Width * image.Height;
        if (orientedPixels > maxPixels)
        {
            var factor = Math.Sqrt((double)maxPixels / orientedPixels);
            var width = Math.Max(1, (int)Math.Floor(image.Width * factor));
            var height = Math.Max(1, (int)Math.Floor(image.Height * factor));
            image.Mutate(x => x.Resize(width, height));
        }

        var result = new MemoryStream();
        image.Save(result, new JpegEncoder { Quality = 95 });
        result.Seek(0, SeekOrigin.Begin);
        return result;
    }

}