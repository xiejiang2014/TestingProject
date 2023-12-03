using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfMenus.Converters
{
    internal class MenuContentTemplateFinderConverter : IValueConverter
    {
        private static MenuContentTemplateFinderConverter? _default;

        public static MenuContentTemplateFinderConverter Default =>
            _default ??= new MenuContentTemplateFinderConverter();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ContentControl { DataContext: MenuItemViewModel menuItemViewModel } contentControl)
            {
                object? r = null;

                if (!string.IsNullOrWhiteSpace(menuItemViewModel.ItemTemplateKey))
                {
                    r = contentControl.TryFindResource(menuItemViewModel.ItemTemplateKey);
                }

                return r ??= contentControl.FindResource("DefaultChildrenMenuItemsTemplate");
            }


            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}