#nullable enable

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShareDrawing.Tools.MessageBox.ValueConverter
{
    public class ButtonStyleConverter : IValueConverter
    {
        private static ButtonStyleConverter? _default;
        public static ButtonStyleConverter Default=> _default ??= new ButtonStyleConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                var style = boolValue
                    ? (Style?)System.Windows.Application.Current.TryFindResource("MessageBoxAccentButtonStyle")
                    : (Style?)System.Windows.Application.Current.TryFindResource("MessageBoxNonAccentButtonStyle");

                if (style is not null)
                {
                    return style;
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}