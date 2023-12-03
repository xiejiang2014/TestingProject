using System.Windows;
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
        var stylePath = new PropertyPath("Style");
        var styleBinding = new Binding()
        {
            Path            = stylePath,
            TargetNullValue = FindResource(typeof(WaitingBox))
        };

        SetBinding(StyleProperty, styleBinding);

        base.OnApplyTemplate();
    }
}