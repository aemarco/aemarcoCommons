using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;

namespace aemarcoCommons.WpfTools.Extensions;

public static class ImageExtensions
{
    public static ImageSource? ToImageSource(this System.Drawing.Image? image)
    {
        if (image == null)
            return null;

        var memoryStream = new MemoryStream();
        image.Save(memoryStream, ImageFormat.Jpeg);
        memoryStream.Seek(0, SeekOrigin.Begin);
        var result = memoryStream.ToImageSource();
        return result;
    }
    public static ImageSource? ToImageSource(this Stream? image)
    {
        if (image == null)
            return null;

        var bitmap = new System.Windows.Media.Imaging.BitmapImage();
        bitmap.BeginInit();
        bitmap.StreamSource = image;
        bitmap.EndInit();
        return bitmap;
    }

}