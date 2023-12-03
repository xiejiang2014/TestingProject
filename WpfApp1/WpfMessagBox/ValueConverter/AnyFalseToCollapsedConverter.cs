using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WpfMessageBox.ValueConverter;

public class AnyFalseToCollapsedConverter : IMultiValueConverter
{
    private static AnyFalseToCollapsedConverter? _default;

    public static AnyFalseToCollapsedConverter Default =>
        _default ??= new AnyFalseToCollapsedConverter();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.Any(v => v is false) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}