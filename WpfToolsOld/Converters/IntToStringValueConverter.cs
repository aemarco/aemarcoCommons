using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfToolsOld.Converters
{
    /// <summary>
    /// Two-way converter from int to string
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    [Obsolete]
    public class IntToStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intType = (int)value;
            return intType.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            if (int.TryParse(strValue, out int resultInt))
            {
                return resultInt;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
