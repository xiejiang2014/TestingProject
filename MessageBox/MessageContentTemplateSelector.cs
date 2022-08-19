using System.Windows;
using System.Windows.Controls;

namespace ShareDrawing.Tools.MessageBox
{
    internal class MessageContentTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // ReSharper disable once ExpressionIsAlwaysNull
            if (item == null) return base.SelectTemplate(item, container);

            if (!(container is FrameworkElement frameworkElement))
                return base.SelectTemplate(item, container);

            if (!(item is MessageBoxViewModel messageBoxViewModel))
                return base.SelectTemplate(item, container);

            return messageBoxViewModel.MessageBoxType switch
            {
                MessageBoxTypes.Waiting => (frameworkElement.FindResource("WaitingMessageTemplate") as DataTemplate),
                MessageBoxTypes.TextMessage => (frameworkElement.FindResource("TextMessageTemplate") as DataTemplate),
                MessageBoxTypes.Customize => (frameworkElement.FindResource("CustomizeTemplate") as DataTemplate),
                MessageBoxTypes.CustomizeWithButton => (frameworkElement.FindResource("CustomizeWithButtonTemplate") as DataTemplate),
                _ => base.SelectTemplate(item, container)
            };
        }
    }
}