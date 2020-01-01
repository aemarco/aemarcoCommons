using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfToolsOld.Converters
{
    /// <summary>
    /// Two-way converter from bool to Visibility // false means visible 
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InversBoolToVisibilityValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            if (Enum.TryParse<Visibility>(strValue, out Visibility result))
            {
                return result;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
