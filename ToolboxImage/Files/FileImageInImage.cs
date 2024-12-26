namespace aemarcoCommons.ToolboxImage.Files;

public class FileImageInImage : ImageInImage, IFileImageInImage
{
    public FileImageInImage(Rectangle targetArea, string filePath)
        : base(targetArea)
    {
        FilePath = filePath;
    }
    public string FilePath { get; }
}