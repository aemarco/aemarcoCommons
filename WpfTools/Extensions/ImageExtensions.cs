using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
#nullable enable

namespace aemarcoCommons.WpfTools.Extensions;

public static class ImageExtensions
{
    public static ImageSource? ToImageSource(this System.Drawing.Image? image, ImageFormat? format = null)
    {
        if (image == null)
            return null;

        var bitmap = new System.Windows.Media.Imaging.BitmapImage();
        bitmap.BeginInit();
        var memoryStream = new MemoryStream();
        image.Save(memoryStream, format ?? ImageFormat.Jpeg);
        memoryStream.Seek(0, SeekOrigin.Begin);
        bitmap.StreamSource = memoryStream;
        bitmap.EndInit();
        return bitmap;

    }


}