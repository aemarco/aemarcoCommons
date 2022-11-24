using Humanizer;
using Humanizer.Localisation;
using System;
using System.Globalization;
using System.Windows.Data;


namespace aemarcoCommons.WpfTools.Converters;

[ValueConversion(typeof(TimeSpan?), typeof(string))]
public class TimespanToStringConverter : IValueConverter
{
    /// <summary>
    /// One-way converter from TimeSpan? to string 
    /// </summary>


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (TimeSpan?)value;

        if (!val.HasValue)
            return "Unknown";

        var human = val.Value.Humanize(2, collectionSeparator: " ", minUnit: TimeUnit.Second);
        return val.Value.TotalMilliseconds < 0
            ? $"- {human}"
            : human;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

}