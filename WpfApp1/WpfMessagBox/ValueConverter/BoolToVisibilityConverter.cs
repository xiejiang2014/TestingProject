using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfMessageBox.ValueConverter;

/// <summary>
/// 将bool转换为Visibility
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    private static BoolToVisibilityConverter? _default;
    public static  BoolToVisibilityConverter  Default => _default ??= new BoolToVisibilityConverter();

    private static BoolToVisibilityConverter? _defaultReversed;

    public static BoolToVisibilityConverter DefaultReversed =>
        _defaultReversed ??= new BoolToVisibilityConverter
        (
            false,
            true
        );


    private static BoolToVisibilityConverter? _defaultHidden;

    public static BoolToVisibilityConverter DefaultHidden =>
        _defaultHidden ??= new BoolToVisibilityConverter
        (
            true,
            false
        );

    private static BoolToVisibilityConverter? _defaultHiddenReversed;

    public static BoolToVisibilityConverter DefaultHiddenReversed =>
        _defaultHiddenReversed ??= new BoolToVisibilityConverter
        (
            true,
            true
        );


    public BoolToVisibilityConverter()
    {
    }

    public BoolToVisibilityConverter
    (
        bool useHidden,
        bool isReversed
    )
    {
        UseHidden  = useHidden;
        IsReversed = isReversed;
    }

    /// <summary>
    /// 是否转换为Hidden,否则转换为Collapsed
    /// </summary>
    public bool UseHidden { get; set; }

    /// <summary>
    /// 是否反转,默认为否.
    /// 默认true转换为Visible,false转换为Hidden或Collapsed(由UseHidden决定);
    /// 反转后false转换为Visible,true转换为Hidden或Collapsed(由UseHidden决定)
    /// </summary>
    public bool IsReversed { get; set; }

    public object Convert
    (
        object      value,
        Type        targetType,
        object      parameter,
        CultureInfo culture
    )
    {
        var val = System.Convert.ToBoolean
        (
            value,
            CultureInfo.InvariantCulture
        );

        if (IsReversed) val = !val;

        if (val) return Visibility.Visible;

        return UseHidden ?
            Visibility.Hidden :
            Visibility.Collapsed;
    }

    public object ConvertBack
    (
        object      value,
        Type        targetType,
        object      parameter,
        CultureInfo culture
    )
    {
        if (value is Visibility visibility)
        {
            var result = visibility == Visibility.Visible;

            if (IsReversed)
            {
                result = !result;
            }

            return result;
        }

        return DependencyProperty.UnsetValue;
    }
}