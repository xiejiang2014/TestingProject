using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WpfMenus.Converters
{
    public class MultiParametersConverter : IMultiValueConverter
    {
        private static MultiParametersConverter? _default;

        public static MultiParametersConverter Default
        {
            get
            {
                if (_default is null)
                {
                    _default = new MultiParametersConverter();
                }

                return _default;
            }
        }


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            return values.ToArray();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { value };
        }
    }
}