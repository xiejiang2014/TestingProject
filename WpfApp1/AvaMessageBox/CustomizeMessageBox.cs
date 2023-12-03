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
    ////////static CustomizeMessageBox()
    ////////{
    ////////    DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomizeMessageBox),
    ////////                                             new FrameworkPropertyMetadata(
    ////////                                                 typeof(CustomizeMessageBox)));
    ////////}


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        //todo 样式
        //var stylePath = new PropertyPath(nameof(MessageBoxViewModel.Style));
        //var styleBinding = new Binding()
        //{
        //    Path            = stylePath,
        //    TargetNullValue = FindResource(typeof(CustomizeMessageBox))
        //};

        //SetBinding(StyleProperty, styleBinding);

        //==

        if (e.NameScope.Find("PART_Title") is TextBlock textBlock)
        {
            textBlock.Bind(TextBlock.TextProperty, new Binding(nameof(MessageBoxViewModel.Title)));
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

        if (e.NameScope.Find("PART_ButtonsControl") is ItemsControl itemsControl)
        {
            itemsControl.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(MessageBoxViewModel.ButtonBehaviors)));
        }

        //==
        base.OnApplyTemplate(e);
    }
}