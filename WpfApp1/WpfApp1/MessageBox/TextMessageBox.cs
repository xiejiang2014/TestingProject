using System.Windows;
using System.Windows.Data;

namespace WpfApp1.MessageBox
{
    [TemplatePart(Name = "PART_ButtonOK")]
    [TemplatePart(Name = "PART_ButtonYes")]
    [TemplatePart(Name = "PART_ButtonNo")]
    [TemplatePart(Name = "PART_ButtonCancel")]
    [TemplatePart(Name = "PART_ButtonClose")]
    public class TextMessageBox : MessageBoxBase
    {
        static TextMessageBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextMessageBox),
                new FrameworkPropertyMetadata(typeof(TextMessageBox)));
        }

        public TextMessageBox()
        {
            DataContextChanged += MessageBoxBase_DataContextChanged;
        }

        public override void OnApplyTemplate()
        {
            var stylePath = new PropertyPath("Style");
            var styleBinding = new Binding()
            {
                Path = stylePath,
                TargetNullValue = FindResource(typeof(TextMessageBox))
            };

            SetBinding(StyleProperty, styleBinding);

            base.OnApplyTemplate();
        }
    }
}