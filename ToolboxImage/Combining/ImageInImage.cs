// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace aemarcoCommons.ToolboxImage.Combining;

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

    ~ImageInImage()
    {
        Dispose(false);
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

    #endregion

}