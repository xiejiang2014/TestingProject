using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using WpfApp1.MessageBox.ValueConverter;

namespace WpfApp1.MessageBox
{
    public class MessageBoxBase : Control
    {
        private MessageBoxViewModel? _messageBoxViewModel;

        protected void MessageBoxBase_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is MessageBoxViewModel messageBoxViewModel)
            {
                _messageBoxViewModel = messageBoxViewModel;
            }
        }


        private ButtonBase? _buttonOk;
        private ButtonBase? _buttonYes;
        private ButtonBase? _buttonNo;
        private ButtonBase? _buttonCancel;
        private ButtonBase? _buttonClose;

        public override void OnApplyTemplate()
        {
            if (GetTemplateChild("PART_ButtonOK") is ButtonBase buttonOk)
            {
                _buttonOk = buttonOk;
                _buttonOk.Click += (_, _) => _messageBoxViewModel?.OkButtonBehavior.ClickAction?.Invoke();

                var commandPath = new PropertyPath("OkButtonBehavior.ClickAction");
                BindCommandToButton(_buttonOk, commandPath);
            }


            if (GetTemplateChild("PART_ButtonYes") is ButtonBase buttonYes)
            {
                _buttonYes = buttonYes;
                _buttonYes.Click += (_, _) => _messageBoxViewModel?.YesButtonBehavior.ClickAction?.Invoke();

                var commandPath = new PropertyPath("YesButtonBehavior.ClickAction");
                BindCommandToButton(_buttonYes, commandPath);
            }


            if (GetTemplateChild("PART_ButtonNo") is ButtonBase buttonNo)
            {
                _buttonNo = buttonNo;
                _buttonNo.Click += (_, _) => _messageBoxViewModel?.NoButtonBehavior.ClickAction?.Invoke();

                var commandPath = new PropertyPath("NoButtonBehavior.ClickAction");
                BindCommandToButton(_buttonNo, commandPath);
            }


            if (GetTemplateChild("PART_ButtonCancel") is ButtonBase buttonCancel)
            {
                _buttonCancel = buttonCancel;
                _buttonCancel.Click += (_, _) => _messageBoxViewModel?.CancelButtonBehavior.ClickAction?.Invoke();

                var commandPath = new PropertyPath("CancelButtonBehavior.ClickAction");
                BindCommandToButton(_buttonCancel, commandPath);
            }


            if (GetTemplateChild("PART_ButtonClose") is ButtonBase buttonClose)
            {
                _buttonClose = buttonClose;
                _buttonClose.Click += (_, _) => _messageBoxViewModel?.CloseButtonBehavior.ClickAction?.Invoke();

                var commandPath = new PropertyPath("CloseButtonBehavior.ClickAction");
                BindCommandToButton(_buttonClose, commandPath);
            }


            base.OnApplyTemplate();
        }

        private static void BindCommandToButton(
            ButtonBase button,
            PropertyPath commandPath
        )
        {
            var binding = new Binding()
            {
                Path = commandPath,
                Converter = NullToCollapsedConverter.Default
            };

            button.SetBinding(VisibilityProperty, binding);
        }
    }
}