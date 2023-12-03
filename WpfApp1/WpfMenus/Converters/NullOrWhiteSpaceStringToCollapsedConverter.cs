using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfMenus.Converters
{
    public class NullOrWhiteSpaceStringToCollapsedConverter : IValueConverter
    {
        private static NullOrWhiteSpaceStringToCollapsedConverter? _default;
        public static NullOrWhiteSpaceStringToCollapsedConverter Default => _default ??= new();

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null || value is string stringValue && string.IsNullOrWhiteSpace(stringValue)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}