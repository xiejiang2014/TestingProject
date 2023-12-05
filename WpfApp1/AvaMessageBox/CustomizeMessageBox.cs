using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AvaMessageBox;

[TemplatePart("PART_Title",                   typeof(TextBlock))]
[TemplatePart("PART_ButtonsControl",          typeof(ItemsControl))]
[TemplatePart("PART_CustomizeContentControl", typeof(ContentControl))]
public class CustomizeMessageBox : MessageBoxBase
{
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (e.NameScope.Find("PART_Title") is TextBlock textBlock)
        {
            textBlock.Bind(TextBlock.TextProperty, new Binding(nameof(MessageBoxViewModel.Title)));
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