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

public static class ImageInImageExtensions
{

    public static Image CreateOuterImage(this IImageInImage image) =>
        CreateOuterImage([image]);

    public static Image CreateOuterImage(this IEnumerable<IImageInImage> images)
    {
        IImageInImage[] array = [.. images];
        var width = array.Max(x => x.TargetArea.X + x.TargetArea.Width);
        var height = array.Max(x => x.TargetArea.Y + x.TargetArea.Height);
        Image result = new Image<Rgba32>(width, height);
        foreach (var image in array)
            image.DrawToImage(result);
        return result;
    }

}
