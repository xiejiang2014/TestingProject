using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ShareDrawing.Tools.MessageBox.ValueConverter;

namespace ShareDrawing.Tools.MessageBox
{
    [TemplatePart(Name = "PART_ButtonOK")]
    [TemplatePart(Name = "PART_ButtonYes")]
    [TemplatePart(Name = "PART_ButtonNo")]
    [TemplatePart(Name = "PART_ButtonCancel")]
    [TemplatePart(Name = "PART_ButtonClose")]
    public class TextMessageBox : Control
    {
        private ButtonBase _buttonOk;
        private ButtonBase _buttonYes;
        private ButtonBase _buttonNo;
        private ButtonBase _buttonCancel;
        private ButtonBase _buttonClose;
        private MessageBoxViewModel _messageBoxViewModel;

        private readonly NullToCollapsedConverter _nullToCollapsedConverter = new();

        static TextMessageBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextMessageBox),
                new FrameworkPropertyMetadata(typeof(TextMessageBox)));
        }

        public TextMessageBox()
        {
            DataContextChanged += TextMessageBox_DataContextChanged;
        }

        private void TextMessageBox_DataContextChanged(object sender,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (e.NewValue is MessageBoxViewModel messageBoxViewModel)
            {
                _messageBoxViewModel = messageBoxViewModel;
            }
        }

        public override void OnApplyTemplate()
        {
            _buttonOk = GetTemplateChild("PART_ButtonOK") as ButtonBase;
            if (_buttonOk is not null)
            {
                _buttonOk.Click += (s, e) => _messageBoxViewModel.OkButtonBehavior.ClickAction?.Invoke();

                var commandPath = new PropertyPath("OkButtonBehavior.ClickAction");
                BindCommandToButton(_buttonOk, commandPath);
            }

            _buttonYes = GetTemplateChild("PART_ButtonYes") as ButtonBase;
            if (_buttonYes is not null)
            {
                _buttonYes.Click += (s, e) => _messageBoxViewModel.YesButtonBehavior.ClickAction?.Invoke();

                var commandPath = new PropertyPath("YesButtonBehavior.ClickAction");
                BindCommandToButton(_buttonYes, commandPath);
            }

            _buttonNo = GetTemplateChild("PART_ButtonNo") as ButtonBase;
            if (_buttonNo is not null)
            {
                _buttonNo.Click += (s, e) => _messageBoxViewModel.NoButtonBehavior.ClickAction?.Invoke();

                var commandPath = new PropertyPath("NoButtonBehavior.ClickAction");
                BindCommandToButton(_buttonNo, commandPath);
            }

            _buttonCancel = GetTemplateChild("PART_ButtonCancel") as ButtonBase;
            if (_buttonCancel is not null)
            {
                _buttonCancel.Click += (s, e) => _messageBoxViewModel.CancelButtonBehavior.ClickAction?.Invoke();

                var commandPath = new PropertyPath("CancelButtonBehavior.ClickAction");
                BindCommandToButton(_buttonCancel, commandPath);
            }

            _buttonClose = GetTemplateChild("PART_ButtonClose") as ButtonBase;
            if (_buttonClose is not null)
            {
                _buttonClose.Click += (s, e) => _messageBoxViewModel.CloseButtonBehavior.ClickAction?.Invoke();

                var commandPath = new PropertyPath("CloseButtonBehavior.ClickAction");
                BindCommandToButton(_buttonClose, commandPath);
            }

            base.OnApplyTemplate();
        }

        private static void BindCommandToButton(ButtonBase button,
            PropertyPath commandPath
        )
        {
            if (button != null)
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
}