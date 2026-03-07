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

public static class ImageConvert
{
    /// <summary>
    /// Converts an image loaded from a byte array to JPEG format.
    /// </summary>
    /// <param name="source">The byte array containing the image data.</param>
    /// <returns>A byte array containing the image in JPEG format.</returns>
    public static byte[] ToJpeg(byte[] source)
    {
        using var inputStream = new MemoryStream(source);
        using var image = Image.Load(inputStream);
        
        using var outputStream = new MemoryStream();
        image.Save(outputStream, new JpegEncoder());
        return outputStream.ToArray();
    }
}
