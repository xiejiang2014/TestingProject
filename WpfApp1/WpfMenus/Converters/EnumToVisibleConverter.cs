using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfMenus.Converters
{
    public class IntToVisibleConverter : IValueConverter
    {
        private static IntToVisibleConverter? _default;
        public static IntToVisibleConverter Default { get; } = _default ??= new IntToVisibleConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string sp)
            {
                if (int.TryParse(sp, out var temp))
                {
                    parameter = temp;
                }
            }

            if (value is int e && parameter is int p)
            {
                return e == p ? Visibility.Visible : Visibility.Collapsed;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    public class EnumToVisibleConverter : IValueConverter
    {
        private static EnumToVisibleConverter? _default;
        public static EnumToVisibleConverter Default { get; } = _default ??= new EnumToVisibleConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum e && parameter is Enum p)
            {
                return e.GetHashCode() == p.GetHashCode() ? Visibility.Visible : Visibility.Collapsed;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}