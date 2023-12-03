using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfMessagBox;

[TemplatePart(Name = "PART_Title")]
[TemplatePart(Name = "PART_ButtonsControl")]
[TemplatePart(Name = "PART_CustomizeContentControl")]
public class CustomizeMessageBox : MessageBoxBase
{
    static CustomizeMessageBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomizeMessageBox),
                                                 new FrameworkPropertyMetadata(
                                                     typeof(CustomizeMessageBox)));
    }


    public override void OnApplyTemplate()
    {
        var stylePath = new PropertyPath(nameof(MessageBoxViewModel.Style));
        var styleBinding = new Binding()
        {
            Path            = stylePath,
            TargetNullValue = FindResource(typeof(CustomizeMessageBox))
        };

        SetBinding(StyleProperty, styleBinding);

        //==

        if (GetTemplateChild("PART_Title") is TextBlock textBlock)
        {
            var binding = new Binding()
            {
                Path = new PropertyPath(nameof(MessageBoxViewModel.Title))
            };

            textBlock.SetBinding(TextBlock.TextProperty, binding);
        }

        //==

        //if (GetTemplateChild("PART_CustomizeContentControl") is ContentControl contentControl)
        //{
        //    var binding = new Binding()
        //    {
        //        Path = new PropertyPath(nameof(MessageBoxViewModel.CustomizeContent))
        //    };

        //    contentControl.SetBinding(ContentControl.ContentProperty, binding);
        //}

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