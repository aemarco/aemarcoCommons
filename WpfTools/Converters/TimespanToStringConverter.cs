using System;
using System.Globalization;
using System.Windows.Data;
using aemarcoCommons.Extensions.TimeExtensions;
using Humanizer;
using Humanizer.Localisation;


namespace aemarcoCommons.WpfTools.Converters
{

    [ValueConversion(typeof(TimeSpan?), typeof(string))]
    public class TimespanToStringConverter : IValueConverter
    {
        /// <summary>
        /// One-way converter from TimeSpan? to string 
        /// </summary>


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        { 
            var val = (TimeSpan?)value;
            return !val.HasValue 
                ? "Unknown" 
                : val.Value.Humanize(2, collectionSeparator: " ", minUnit: TimeUnit.Second);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
