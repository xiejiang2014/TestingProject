using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfMessageBox.ValueConverter;

public class IsNullConverter : IValueConverter
{
    private static IsNullConverter? _default;

    public static IsNullConverter Default =>
        _default ??= new IsNullConverter();

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}