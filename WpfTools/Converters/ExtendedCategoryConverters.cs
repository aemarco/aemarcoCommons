using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Contracts.Api.ResponseObjects;
// ReSharper disable UnusedType.Global

namespace WpfTools.Converters
{
    [ValueConversion(typeof(ExtendedCategory), typeof(BitmapImage))]
    public class ExtendedCategoryToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var category = value as ExtendedCategory;
            var result = new BitmapImage();
            if (!string.IsNullOrWhiteSpace(category?.Icon))
            {
                result.BeginInit();
                result.StreamSource = new MemoryStream(System.Convert.FromBase64String(category.Icon));
                result.EndInit();
            }
            return result;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(ExtendedCategory), typeof(string))]
    public class ExtendedCategoryToSpacerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var category = value as ExtendedCategory;
            return (category?.IsMainCategory ?? false) ? string.Empty : "      ";
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
