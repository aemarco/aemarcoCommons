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

namespace aemarcoCommons.ToolboxImage.Configuration;

public record FixedImageInImageSettings
{
    public int X { get; init; }
    public int Y { get; init; }
    public int Width { get; init; } = 1920;
    public int Height { get; init; } = 1080;
    public ImageScaleMode ImageScaleMode { get; init; } = ImageScaleMode.FillAndFit;
    public int MaxHorizontalCropPercentage { get; init; } = 20;
    public int MaxVerticalCropPercentage { get; init; } = 10;
}