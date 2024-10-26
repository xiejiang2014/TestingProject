using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfMessageBox;

[TemplatePart(Name = "PART_Title")]
[TemplatePart(Name = "PART_ButtonsControl")]
[TemplatePart(Name = "PART_ButtonClose")]
[TemplatePart(Name = "PART_CustomizeContentControl")]
public class MessageBox : Control
{
    protected MessageBoxViewModel? MessageBoxViewModel;

    static MessageBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageBox),
                                                 new FrameworkPropertyMetadata(
                                                                               typeof(MessageBox)));
    }

    public MessageBox()
    {
        DataContextChanged += MessageBox_DataContextChanged;
    }

    private void MessageBox_DataContextChanged(object                             sender,
                                                   DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is MessageBoxViewModel messageBoxViewModel)
        {
            MessageBoxViewModel = messageBoxViewModel;
        }
    }

    public override void OnApplyTemplate()
    {
        var stylePath = new PropertyPath(nameof(MessageBoxViewModel.Style));
        var styleBinding = new Binding()
                           {
                               Path            = stylePath,
                               TargetNullValue = FindResource(typeof(MessageBox))
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

        if (GetTemplateChild("PART_CustomizeContentControl") is ContentControl contentControl)
        {
            var binding = new Binding()
                          {
                              Path = new PropertyPath(nameof(MessageBoxViewModel.CustomizeContent))
                          };

            contentControl.SetBinding(ContentControl.ContentProperty, binding);
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