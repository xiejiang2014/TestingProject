using Avalonia.Controls.Primitives;

namespace AvaMessageBox;

public class WaitingBox : MessageBoxBase
{
    //static WaitingBox()
    //{
    //    DefaultStyleKeyProperty.OverrideMetadata(typeof(WaitingBox), new FrameworkPropertyMetadata(typeof(WaitingBox)));
    //}


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        //ÑשÊ½
        //////var stylePath = new PropertyPath("Style");
        //////var styleBinding = new Binding()
        //////{
        //////    Path            = stylePath,
        //////    TargetNullValue = FindResource(typeof(WaitingBox))
        //////};

        //////SetBinding(StyleProperty, styleBinding);

        base.OnApplyTemplate(e);
    }
}