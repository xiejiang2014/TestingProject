using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.MessageBox
{
    public interface ICustomizeContentViewModel : INotifyPropertyChanged
    {
        Action MessageBoxRequestToClose { get; set; }
    }

    public class MessageBoxViewModel : INotifyPropertyChanged
    {
        public MessageBoxTypes MessageBoxType { get; set; }

        public string? Title { get; set; }

        public string? Message { get; set; }

        public UIElement? CustomizeContent { get; set; }


        private ICustomizeContentViewModel? _customizeContentViewModel;

        public ICustomizeContentViewModel? CustomizeContentViewModel
        {
            get => _customizeContentViewModel;
            set
            {
                if (_customizeContentViewModel is not null)
                {
                    _customizeContentViewModel.MessageBoxRequestToClose -= MessageBoxRequestToClose;
                }

                _customizeContentViewModel = value;

                if (_customizeContentViewModel is not null)
                {
                    _customizeContentViewModel.MessageBoxRequestToClose += MessageBoxRequestToClose;
                }
            }
        }

        private void MessageBoxRequestToClose()
        {
            if (_customizeContentViewModel is not null)
            {
                CustomizeContentViewModel.MessageBoxRequestToClose -= MessageBoxRequestToClose;
            }

            if (CloseButtonBehavior.CanExecute)
            {
                CloseButtonBehavior.ClickAction?.Invoke();
            }
        }

        public MessageBoxResults Result { get; set; } = MessageBoxResults.None;

        public MessageBoxViewModel()
        {
            OkButtonBehavior.ButtonContent = OkButtonDefaultContent;
            YesButtonBehavior.ButtonContent = YesButtonDefaultContent;
            NoButtonBehavior.ButtonContent = NoButtonDefaultContent;
            CancelButtonBehavior.ButtonContent = CancelButtonDefaultContent;
            CloseButtonBehavior.ButtonContent = CloseButtonDefaultContent;
        }

        #region 样式

        public Style? Style { get; set; }

        #endregion

        #region 布局

        public Thickness Margin { get; set; } = new(0, 0, 0, 0);
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;

        #endregion


        #region Ok

        public ButtonBehavior OkButtonBehavior { get; set; } = new();
        public static object OkButtonDefaultContent { get; set; } = "确定";

        #endregion

        #region Yes

        public ButtonBehavior YesButtonBehavior { get; set; } = new();
        public static object YesButtonDefaultContent { get; set; } = "是";

        #endregion

        #region No

        public ButtonBehavior NoButtonBehavior { get; set; } = new();
        public static object NoButtonDefaultContent { get; set; } = "否";

        #endregion

        #region Cancel

        public ButtonBehavior CancelButtonBehavior { get; set; } = new();

        public static object CancelButtonDefaultContent { get; set; } = "取消";

        /// <summary>
        /// 是否已取消.此属性在等待框中有效.
        /// </summary>
        public bool IsCanceled { get; set; }

        #endregion

        #region Close

        public bool CloseWhenMaskMouseLeftButtonDown { get; set; } = false;
        public ButtonBehavior CloseButtonBehavior { get; } = new();
        public static object CloseButtonDefaultContent { get; set; } = "关闭";


        /// <summary>
        /// 是否已关闭
        /// </summary>
        public bool IsClosed { get; internal set; }

        #endregion

        public Task WaitMessageBoxClose()
        {
            return Task.Run(() =>
            {
                while (!IsClosed)
                {
                    Thread.Sleep(1);
                }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}