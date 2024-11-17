using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace aemarcoCommons.WpfTools.Converters;

/// <summary>
/// One-way converter from System.Drawing.Image to System.Windows.Media.ImageSource
/// </summary>
[ValueConversion(typeof(System.Drawing.Image), typeof(System.Windows.Media.ImageSource))]
public class ImageToImageSource : IValueConverter
{
    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        // empty images are empty...
        if (value == null)
            return null;


        var image = (System.Drawing.Image)value;

        var bitmap = new System.Windows.Media.Imaging.BitmapImage();
        bitmap.BeginInit();
        MemoryStream memoryStream = new MemoryStream();

        image.Save(memoryStream, ImageFormat.Bmp);

        memoryStream.Seek(0, SeekOrigin.Begin);
        bitmap.StreamSource = memoryStream;
        bitmap.EndInit();
        return bitmap;
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}