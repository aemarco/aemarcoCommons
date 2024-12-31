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

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace aemarcoCommons.ToolboxImage.Model;

public class ImageInImage : IImageInImage
{

    public ImageInImage(Rectangle targetArea)
    {
        TargetArea = targetArea;
        Image = new Image<Rgba32>(TargetArea.Width, TargetArea.Height);
        Timestamp = DateTimeOffset.MinValue;
        ChangedSinceDrawn = false;
        _mutatedImage = Image;
    }
    public Rectangle TargetArea { get; }
    public Image Image { get; private set; }
    public DateTimeOffset Timestamp { get; private set; }
    public bool ChangedSinceDrawn { get; private set; }


    private ImageScaleMode _imageScaleMode;
    private int _maxVerticalCropPercentage;
    private int _maxHorizontalCropPercentage;
    public virtual void SetImage(
        Image image,
        ImageScaleMode mode,
        int maxHorizontalCropPercentage,
        int maxVerticalCropPercentage)
    {
        Image = image;
        Timestamp = DateTimeOffset.Now;
        ChangedSinceDrawn = true;

        _imageScaleMode = mode;
        _maxHorizontalCropPercentage = maxHorizontalCropPercentage;
        _maxVerticalCropPercentage = maxVerticalCropPercentage;
    }

    private Image _mutatedImage;
    public void DrawToImage(Image image)
    {
        if (ChangedSinceDrawn)
        {
            _mutatedImage = Image.ResizeClone(
                TargetArea.Width,
                TargetArea.Height,
                _imageScaleMode,
                _maxHorizontalCropPercentage,
                _maxVerticalCropPercentage);
            ChangedSinceDrawn = false;
        }
        image.Mutate(x => x
            .DrawImage(_mutatedImage, new Point(TargetArea.X, TargetArea.Y), 1f)
        );
    }

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    protected virtual void Dispose(bool disposing)
    {
        //release unmanaged resources here

        if (disposing)
        {
            _mutatedImage.Dispose();
            Image.Dispose();
        }
    }

    ~ImageInImage()
    {
        Dispose(false);
    }
    #endregion

}