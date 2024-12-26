namespace aemarcoCommons.ToolboxImage.Files;

public class FileFixedImageInImage : FileImageInImage, IFileFixedImageInImage
{
    private readonly FileFixedImageInImageSettings _settings;
    public FileFixedImageInImage(
        FileFixedImageInImageSettings settings)
        : base(new Rectangle(settings.X, settings.Y, settings.Width, settings.Height), settings.FilePath)
    {
        _settings = settings;
    }

    public void SetImage(Image image)
    {
        base.SetImage(
            image,
            _settings.ImageScaleMode,
            _settings.MaxHorizontalCropPercentage,
            _settings.MaxVerticalCropPercentage);
    }
}