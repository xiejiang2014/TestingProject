using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfMessagBox;

[TemplatePart(Name = "PART_Title")]
[TemplatePart(Name = "PART_MessageTextBlock")]
[TemplatePart(Name = "PART_ButtonsControl")]
public class TextMessageBox : MessageBoxBase
{
    static TextMessageBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TextMessageBox),
                                                 new FrameworkPropertyMetadata(typeof(TextMessageBox)));
    }

    public override void OnApplyTemplate()
    {
        var stylePath = new PropertyPath(nameof(MessageBoxViewModel.Style));
        var styleBinding = new Binding()
        {
            Path            = stylePath,
            TargetNullValue = FindResource(typeof(TextMessageBox))
        };

        SetBinding(StyleProperty, styleBinding);

        //==

        if (GetTemplateChild("PART_Title") is TextBlock titleTextBlock)
        {
            var binding = new Binding()
            {
                Path = new PropertyPath(nameof(MessageBoxViewModel.Title))
            };

            titleTextBlock.SetBinding(TextBlock.TextProperty, binding);
        }

        //==

        if (GetTemplateChild("PART_MessageTextBlock") is TextBlock messagetextBlock)
        {
            var binding = new Binding()
            {
                Path = new PropertyPath(nameof(MessageBoxViewModel.Message))
            };

            messagetextBlock.SetBinding(TextBlock.TextProperty, binding);
        }

        //==

        if (GetTemplateChild("PART_ButtonsControl") is ItemsControl itemsControl)
        {
            var binding = new Binding()
            {
                Path = new PropertyPath(nameof(MessageBoxViewModel.ButtonBehaviors))
            };

            itemsControl.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }

        //==

        base.OnApplyTemplate();
    }
}