using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfApp1.MessageBox.ValueConverter
{
    public class NullToVisibleConverter : IValueConverter
    {
        private static NullToVisibleConverter? _default;
        public static NullToVisibleConverter Default => _default ??= new NullToVisibleConverter();

        public object Convert(object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            return value is null ? Visibility.Visible : Visibility.Collapsed;
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