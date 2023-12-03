using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace AvaMessageBox.ValueConverter;

public class AllFalseConverter : IMultiValueConverter
{
    private static AllFalseConverter? _default;

    public static AllFalseConverter Default =>
        _default ??= new AllFalseConverter();


    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values.All(v => v is false);
    }
}