using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfMessageBox;

public class WaitingBox : MessageBoxBase
{
    static WaitingBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WaitingBox), new FrameworkPropertyMetadata(typeof(WaitingBox)));
    }


    public override void OnApplyTemplate()
    {
        var stylePath = new PropertyPath(nameof(MessageBoxViewModel.Style));
        var styleBinding = new Binding()
                           {
                               Path            = stylePath,
                               TargetNullValue = FindResource(typeof(WaitingBox))
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