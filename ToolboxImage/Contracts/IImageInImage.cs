namespace aemarcoCommons.ToolboxImage.Contracts;

public interface IImageInImage : IDisposable
{
    Rectangle TargetArea { get; }

    Image Image { get; }
    DateTimeOffset Timestamp { get; }
    bool ChangedSinceDrawn { get; }

    public void SetImage(
        Image image,
        ImageScaleMode mode,
        int maxHorizontalCropPercentage,
        int maxVerticalCropPercentage);

    void DrawToImage(Image image);
}