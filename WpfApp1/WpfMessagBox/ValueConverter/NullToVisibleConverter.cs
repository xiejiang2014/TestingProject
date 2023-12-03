using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfMessagBox.ValueConverter;

public class NullToVisibleConverter : IValueConverter
{
    private static NullToVisibleConverter? _default;

    public static NullToVisibleConverter Default =>
        _default ??= new NullToVisibleConverter();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool.TryParse(parameter?.ToString(), out var reverse);
        if (!reverse)
        {
            return value is null ? Visibility.Visible : Visibility.Collapsed;
        }
        else
        {
            return value is null ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}