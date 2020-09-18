using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace aemarcoCommons.WpfTools.Converters
{
    /// <summary>
    /// Two-way converter from double to string
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleType = (double)value;
            return doubleType.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            if (double.TryParse(strValue, out double resultDouble))
            {
                if (parameter is string strParameter && !String.IsNullOrWhiteSpace(strParameter))
                {
                    var p = new DoubleToStringConverterParameter(strParameter);
                    if (p.Min != null && resultDouble < p.Min)
                        return DependencyProperty.UnsetValue;
                    if (p.Max != null && resultDouble > p.Max)
                        return DependencyProperty.UnsetValue;
                }
                return resultDouble;

            }
            return DependencyProperty.UnsetValue;
        }
    }

    internal class DoubleToStringConverterParameter
    {

        public DoubleToStringConverterParameter(string parameters)
        {
            // min=0.1,max=100
            CultureInfo culture = CultureInfo.InvariantCulture;
            parameters = parameters.ToLower();
            string[] vals = parameters.Split(',');
            foreach (var val in vals)
            {
                if (val.StartsWith("min="))
                {
                    double.TryParse(val.Replace("min=", string.Empty), NumberStyles.AllowDecimalPoint, culture, out double min);
                    Min = min;
                }
                else if (val.StartsWith("max="))
                {
                    double.TryParse(val.Replace("max=", string.Empty), NumberStyles.AllowDecimalPoint, culture, out double max);
                    Max = max;
                }
            }
        }

        public double? Min { get; set; }
        public double? Max { get; set; }

    }
}
