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

namespace aemarcoCommons.ToolboxImage.Model.Enums;

public enum ImageScaleMode
{
    /// <summary>
    /// Crops the image to fill the target area.
    /// If the image's aspect ratio is compatible, it will crop and fill the area.
    /// If not, it will crop as much as allowed, with black bars filling the remaining space.
    /// </summary>
    FillAndFit,
    /// <summary>
    /// Crops the image to completely fill the target area, while preserving the aspect ratio.
    /// Any part of the image outside the bounds of the target area is cropped out.
    /// </summary>
    Fill,
    /// <summary>
    /// Scales the image as large as possible while preserving its aspect ratio, without cropping.
    /// Black bars will be added to the sides or top and bottom if the aspect ratio does not match the target.
    /// </summary>
    Fit

}
