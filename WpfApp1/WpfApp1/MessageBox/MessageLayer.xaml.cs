#nullable enable

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1.MessageBox
{
    /// <summary>
    /// MessageLayer.xaml 的交互逻辑
    /// </summary>
    public partial class MessageLayer : INotifyPropertyChanged
    {
        public MessageLayer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 当前正显示的对话框view的viewModel
        /// </summary>
        public MessageBoxViewModel? MessageBoxViewModel { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void MessageLayer_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void MessageLayer_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBoxViewModel is null)
            {
                return;
            }


            if (MessageBoxViewModel.CloseWhenMaskMouseLeftButtonDown &&
                MessageBoxViewModel.CloseButtonBehavior.CanExecute)
            {
                MessageBoxViewModel.CloseButtonBehavior.ClickAction?.Invoke();
            }
        }

        private void MessageLayerRootContentControl_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void MessageLayer_OnLoaded(object sender, RoutedEventArgs e)
        {
            MessageLayerRootContentControl.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}