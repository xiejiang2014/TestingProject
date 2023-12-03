using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfMenus.Converters;

/// <summary>
/// 对传入的  Thickness 的左边进行修改操作的转换器
/// </summary>
[ValueConversion(typeof(Thickness), typeof(Thickness), ParameterType = typeof(double))]
public class ThicknessLeftSideModifyConverter : IValueConverter
{
    private static ThicknessLeftSideModifyConverter? _default;

    public static ThicknessLeftSideModifyConverter Default =>
        _default ??= new ThicknessLeftSideModifyConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Thickness thickness && parameter is double doubleParameter)
        {
            return new Thickness(thickness.Left + doubleParameter,
                                 thickness.Top,
                                 thickness.Right,
                                 thickness.Bottom
            );
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}

/// <summary>
/// 对传入的  Thickness 的左边进行修改操作的转换器
/// </summary>
[ValueConversion(typeof(int), typeof(Thickness), ParameterType = typeof(Thickness))]
public class IntAttchToThicknessLeftSideConverter : IValueConverter
{
    private static IntAttchToThicknessLeftSideConverter? _default;

    public static IntAttchToThicknessLeftSideConverter Default =>
        _default ??= new IntAttchToThicknessLeftSideConverter();


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is Thickness thickness && value is int intParameter)
        {
            return new Thickness(
                thickness.Left + intParameter,
                thickness.Top,
                thickness.Right,
                thickness.Bottom
            );
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}