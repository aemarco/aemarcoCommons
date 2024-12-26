namespace aemarcoCommons.ToolboxImage.Files;

public record FileFixedImageInImageSettings
{
    public int X { get; init; } = 0;
    public int Y { get; init; } = 0;
    public int Width { get; init; } = 1920;
    public int Height { get; init; } = 1080;
    public required string FilePath { get; init; }
    public ImageScaleMode ImageScaleMode { get; init; } = ImageScaleMode.FillAndFit;
    public int MaxHorizontalCropPercentage { get; init; } = 20;
    public int MaxVerticalCropPercentage { get; init; } = 20;
}