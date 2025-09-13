using System;
using System.Globalization;
using System.Windows.Data;

namespace aemarcoCommons.WpfTools.Converters;

public class Multiply : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var format = new NumberFormatInfo { NumberDecimalSeparator = "." };
        var result =
            double.TryParse(value?.ToString(), NumberStyles.Any, format, out var num1) &&
            double.TryParse(parameter?.ToString(), NumberStyles.Any, format, out var num2)
                ? num1 * num2
                : 0;
        return result;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}