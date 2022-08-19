using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.MessageBox
{
    internal class MessageContentTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return base.SelectTemplate(item, container);
            }

            if (container is not FrameworkElement frameworkElement)
            {
                return base.SelectTemplate(item, container);
            }

            if (item is not MessageBoxViewModel messageBoxViewModel)
            {
                return base.SelectTemplate(item, container);
            }

            var templateObject = messageBoxViewModel.MessageBoxType switch
            {
                MessageBoxTypes.Waiting => (frameworkElement.FindResource("WaitingMessageTemplate")),
                MessageBoxTypes.TextMessage => (frameworkElement.FindResource("TextMessageTemplate")),
                MessageBoxTypes.Customize => (frameworkElement.FindResource("CustomizeTemplate")),
                MessageBoxTypes.CustomizeWithButton => (frameworkElement.FindResource("CustomizeWithButtonTemplate")),
                _ => base.SelectTemplate(item, container)
            };


            if (templateObject is DataTemplate dataTemplate)
            {
                return dataTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}