using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfMenus
{
    internal class MenuItemTemplateSelector : DataTemplateSelector
    {
        private static MenuItemTemplateSelector? _default;

        public static MenuItemTemplateSelector Default =>
            _default ??= new MenuItemTemplateSelector();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not MenuItemViewModel menuItemViewModel)
            {
                throw new ArgumentException($"传入的 {nameof(item)} 必须为 {nameof(MenuItemViewModel)} 类型.");
            }

            if (container is not FrameworkElement frameworkElement)
            {
                throw new ArgumentException($"传入的 {nameof(container)} 必须为 {nameof(FrameworkElement)} 类型.");
            }

            DataTemplate? result = null;

            if (!string.IsNullOrWhiteSpace(menuItemViewModel.TemplateKey) &&
                frameworkElement.TryFindResource(menuItemViewModel.TemplateKey) is DataTemplate dataTemplate)
            {
                result = dataTemplate;
            }

            if (result is null &&
                frameworkElement.TryFindResource("DefaultMenuItemTemplate") is DataTemplate dataTemplate2)
            {
                result = dataTemplate2;
            }

            if (result is null)
            {
                throw new ApplicationException("没有找到菜单项的模版.");
            }

            return result;
        }
    }
}