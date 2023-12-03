﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WpfMessagBox.ValueConverter;

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


public class AnyTrueToCollapsedConverter : IMultiValueConverter
{
    private static AnyTrueToCollapsedConverter? _default;

    public static AnyTrueToCollapsedConverter Default =>
        _default ??= new AnyTrueToCollapsedConverter();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.Any(v => v is true) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}