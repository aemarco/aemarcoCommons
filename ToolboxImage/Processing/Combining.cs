namespace aemarcoCommons.ToolboxImage.Processing;

public static class Combining
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

    public static string CreateOuterImageFile(this IFileImageInImage image) =>
        CreateOuterImageFiles([image]).First();


    //TODO: probably return unchanged files also, and info about if updated or not ?
    public static IEnumerable<string> CreateOuterImageFiles(this IEnumerable<IFileImageInImage> images)
    {
        var groups = images
            .GroupBy(x => x.FilePath)
            .ToDictionary(x => x.Key, x => x.ToArray());

        foreach (var (groupFilePath, groupImages) in groups)
        {
            if (groupImages.All(x => !x.ChangedSinceDrawn))
                continue;

            using var image = groupImages.CreateOuterImage();
            image.SaveAsJpeg(groupFilePath);
            yield return groupFilePath;
        }
    }



}
