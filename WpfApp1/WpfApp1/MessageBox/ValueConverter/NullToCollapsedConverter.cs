using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfApp1.MessageBox.ValueConverter
{
    public class NullToCollapsedConverter : IValueConverter
    {
        private static NullToCollapsedConverter? _default;
        public static NullToCollapsedConverter Default => _default ??= new NullToCollapsedConverter();

        public object Convert(
            object? value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            return value is null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            return Binding.DoNothing;
        }
    }
}