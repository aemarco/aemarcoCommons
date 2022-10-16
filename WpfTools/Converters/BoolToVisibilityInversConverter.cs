using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace aemarcoCommons.WpfTools.Converters;

/// <summary>
/// Two-way converter from bool to Visibility // false means visible 
/// </summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolToVisibilityInversConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string strValue && Enum.TryParse(strValue, out Visibility result))
        {
            return result;
        }
        return DependencyProperty.UnsetValue;
    }
}