////////#nullable enable

////////using System;
////////using System.Globalization;
////////using System.Windows;
////////using System.Windows.Data;

////////namespace WpfMessagBox.MessageBox.ValueConverter;

////////public class ButtonStyleConverter : IValueConverter
////////{
////////    private static ButtonStyleConverter? _default;
////////    public static  ButtonStyleConverter  Default => _default ??= new ButtonStyleConverter();

////////    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
////////    {
////////        if (value is bool boolValue)
////////        {
////////            var style = boolValue
////////                ? (Style?) Application.Current.TryFindResource("MessageBoxAccentButtonStyle")
////////                : (Style?) Application.Current.TryFindResource("MessageBoxNonAccentButtonStyle");

////////            if (style is not null)
////////            {
////////                return style;
////////            }
////////        }
////////        else if (value is ButtonBehavior buttonBehavior)
////////        {
////////            if (buttonBehavior.Style is not null)
////////            {
////////                return buttonBehavior.Style;
////////            }

////////            var style = buttonBehavior.IsAccent
////////                ? (Style?) Application.Current.TryFindResource("MessageBoxAccentButtonStyle")
////////                : (Style?) Application.Current.TryFindResource("MessageBoxNonAccentButtonStyle");

////////            if (style is not null)
////////            {
////////                return style;
////////            }
////////        }


////////        return Binding.DoNothing;
////////    }

////////    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
////////    {
////////        return Binding.DoNothing;
////////    }
////////}