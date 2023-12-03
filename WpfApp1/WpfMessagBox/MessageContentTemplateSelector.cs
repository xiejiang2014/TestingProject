using System.Windows;
using System.Windows.Controls;

namespace WpfMessagBox;

internal class MessageContentTemplateSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (item is null) return base.SelectTemplate(item, container);

        if (container is not FrameworkElement frameworkElement)
            return base.SelectTemplate(item, container);

        if (item is not MessageBoxViewModel messageBoxViewModel)
            return base.SelectTemplate(item, container);

        var result = messageBoxViewModel.MessageBoxType switch
                     {
                         MessageBoxTypes.Waiting     => frameworkElement.FindResource("WaitingMessageTemplate"),
                         MessageBoxTypes.TextMessage => frameworkElement.FindResource("TextMessageTemplate"),
                         MessageBoxTypes.Customize   => frameworkElement.FindResource("CustomizeTemplate"),
                         _                           => base.SelectTemplate(item, container)
                     };

        if (result is DataTemplate dataTemplate)
        {
            return dataTemplate;
        }

        return null;
    }
}