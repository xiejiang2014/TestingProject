using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WpfMessageBox.ValueConverter;

public class AnyVisibleToVisibleConverter : IMultiValueConverter
{
    private static AnyVisibleToVisibleConverter? _default;

    public static AnyVisibleToVisibleConverter Default =>
        _default ??= new AnyVisibleToVisibleConverter();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.Any(v => v is Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}