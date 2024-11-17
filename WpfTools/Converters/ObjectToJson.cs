using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Windows.Data;

namespace aemarcoCommons.WpfTools.Converters;

/// <summary>
/// Two-way converter from bool to Visibility // true means Visible
/// </summary>
[ValueConversion(typeof(object), typeof(string))]
public class ObjectToJson : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null
            ? JsonConvert.SerializeObject(value, Formatting.Indented)
            : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null
            ? JsonConvert.DeserializeObject((string)value, targetType)
            : null;
    }
}