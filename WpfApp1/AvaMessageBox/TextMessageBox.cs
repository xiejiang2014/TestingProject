using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AvaMessageBox;

[TemplatePart("PART_Title",            typeof(TextBlock))]
[TemplatePart("PART_MessageTextBlock", typeof(TextBlock))]
[TemplatePart("PART_ButtonsControl",   typeof(ItemsControl))]
public class TextMessageBox : MessageBoxBase
{
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        //==

        if (e.NameScope.Find("PART_Title") is TextBlock textBlock)
        {
            textBlock.Bind(TextBlock.TextProperty, new Binding(nameof(MessageBoxViewModel.Title)));
        }

        //==
        
        if (e.NameScope.Find("PART_MessageTextBlock") is TextBlock messageTextBlock)
        {
            messageTextBlock.Bind(TextBlock.TextProperty, new Binding(nameof(MessageBoxViewModel.Message)));
        }

        //==

        if (e.NameScope.Find("PART_ButtonsControl") is ItemsControl itemsControl)
        {
            itemsControl.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(MessageBoxViewModel.ButtonBehaviors)));
        }

        //==

        base.OnApplyTemplate(e);
    }
}