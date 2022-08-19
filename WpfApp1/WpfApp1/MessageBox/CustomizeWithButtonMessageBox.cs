using System.Windows;
using System.Windows.Data;

namespace WpfApp1.MessageBox
{
    [TemplatePart(Name = "PART_ButtonOK")]
    [TemplatePart(Name = "PART_ButtonYes")]
    [TemplatePart(Name = "PART_ButtonNo")]
    [TemplatePart(Name = "PART_ButtonCancel")]
    [TemplatePart(Name = "PART_ButtonClose")]
    public class CustomizeWithButtonMessageBox : MessageBoxBase
    {
        static CustomizeWithButtonMessageBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomizeWithButtonMessageBox),
                new FrameworkPropertyMetadata(typeof(CustomizeWithButtonMessageBox)));
        }

        public CustomizeWithButtonMessageBox()
        {
            DataContextChanged += MessageBoxBase_DataContextChanged;
        }


        public override void OnApplyTemplate()
        {
            var stylePath = new PropertyPath("Style");
            var styleBinding = new Binding()
            {
                Path = stylePath,
                TargetNullValue = FindResource(typeof(CustomizeWithButtonMessageBox))
            };

            SetBinding(StyleProperty, styleBinding);

            base.OnApplyTemplate();
        }

        //public override void OnApplyTemplate()
        //{
        //    var stylePath = new PropertyPath("Style");
        //    var styleBinding = new Binding()
        //    {
        //        Path = stylePath,
        //        TargetNullValue = FindResource(typeof(CustomizeWithButtonMessageBox))
        //    };

        //    SetBinding(StyleProperty, styleBinding);


        //    _buttonOk = GetTemplateChild("PART_ButtonOK") as ButtonBase;
        //    if (_buttonOk is not null)
        //    {
        //        _buttonOk.Click += (s, e) => _messageBoxViewModel.OkButtonBehavior.ClickAction?.Invoke();

        //        var commandPath = new PropertyPath("OkButtonBehavior.ClickAction");
        //        BindCommandToButton(_buttonOk, commandPath);
        //    }

        //    _buttonYes = GetTemplateChild("PART_ButtonYes") as ButtonBase;
        //    if (_buttonYes is not null)
        //    {
        //        _buttonYes.Click += (s, e) => _messageBoxViewModel.YesButtonBehavior.ClickAction?.Invoke();

        //        var commandPath = new PropertyPath("YesButtonBehavior.ClickAction");
        //        BindCommandToButton(_buttonYes, commandPath);
        //    }

        //    _buttonNo = GetTemplateChild("PART_ButtonNo") as ButtonBase;
        //    if (_buttonNo is not null)
        //    {
        //        _buttonNo.Click += (s, e) => _messageBoxViewModel.NoButtonBehavior.ClickAction?.Invoke();

        //        var commandPath = new PropertyPath("NoButtonBehavior.ClickAction");
        //        BindCommandToButton(_buttonNo, commandPath);
        //    }

        //    _buttonCancel = GetTemplateChild("PART_ButtonCancel") as ButtonBase;
        //    if (_buttonCancel is not null)
        //    {
        //        _buttonCancel.Click += (s, e) => _messageBoxViewModel.CancelButtonBehavior.ClickAction?.Invoke();

        //        var commandPath = new PropertyPath("CancelButtonBehavior.ClickAction");
        //        BindCommandToButton(_buttonCancel, commandPath);
        //    }

        //    _buttonClose = GetTemplateChild("PART_ButtonClose") as ButtonBase;
        //    if (_buttonClose is not null)
        //    {
        //        _buttonClose.Click += (s, e) => _messageBoxViewModel.CloseButtonBehavior.ClickAction?.Invoke();

        //        var commandPath = new PropertyPath("CloseButtonBehavior.ClickAction");
        //        BindCommandToButton(_buttonClose, commandPath);
        //    }

        //    base.OnApplyTemplate();
        //}


        //private static void BindCommandToButton(ButtonBase button,
        //    PropertyPath commandPath
        //)
        //{
        //    if (button != null)
        //    {
        //        var binding = new Binding()
        //        {
        //            Path = commandPath,
        //            Converter = NullToCollapsedConverter.Default
        //        };

        //        button.SetBinding(VisibilityProperty, binding);
        //    }
        //}
    }
}