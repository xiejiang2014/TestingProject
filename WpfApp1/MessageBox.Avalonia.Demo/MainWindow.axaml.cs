using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using MessageBox.Avalonia;

namespace MessageBox.Avalonia.Demo
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel MainWindowViewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext            = MainWindowViewModel;
            ContentControl.Content = MessageBoxManager.Default;
        }

        private void ButtonBase_OnClick(object? sender, RoutedEventArgs e)
        {
            var messageBoxViewModel = new MessageBoxViewModel()
                                      {
                                          Message = "文本消息",
                                          Title   = "自定义标题",
                                      };

            MessageBoxManager.Default.QuickSetButtons(messageBoxViewModel,
                                                      MessageButtonTypes.YesNoCancel,
                                                      isCloseButtonVisible: true);


            MessageBoxManager.Default.ShowMessageBox(messageBoxViewModel);
        }

        private void ButtonBase2_OnClick(object? sender, RoutedEventArgs e)
        {
            var userContent = new UserContent();

            var messageBoxViewModel = new MessageBoxViewModel()
                                      {
                                          Message          = "文本消息",
                                          Title            = "自定义标题",
                                          CustomizeContent = userContent
                                      };


            MessageBoxManager.Default.QuickSetButtons(messageBoxViewModel,
                                                      MessageButtonTypes.NoButton,
                                                      isCloseButtonVisible: true);


            messageBoxViewModel.HorizontalContentAlignment = HorizontalAlignment.Center;
            messageBoxViewModel.VerticalContentAlignment   = VerticalAlignment.Center;

            messageBoxViewModel.ButtonBehaviors.Add(new ButtonBehavior()
                                                    {
                                                        ButtonContent = "自定义按钮",
                                                        ClickAction   = () => { MessageBoxManager.Default.CloseMessageBox(messageBoxViewModel); },
                                                        //Style = Application.Current.FindResource("ExtButtonStyle") as Style
                                                    });

            //messageBoxViewModel.CloseButtonBehavior.ClickAction = () =>
            //                                                      {
            //                                                          MessageBoxManager.Default.CloseMessageBoxWidthDefaultClosingAnimation(messageBoxViewModel);
            //                                                      };


            MessageBoxManager.Default.ShowMessageBox(messageBoxViewModel);
        }

        private void ButtonBase4_OnClick(object? sender, RoutedEventArgs e)
        {
            MainWindowViewModel.Opacity = 1;
        }

        private void ButtonBase3_OnClick(object? sender, RoutedEventArgs e)
        {
            var messageBoxViewModel = new MessageBoxViewModel()
                                      {
                                          Message           = "Message",
                                          Title             = "Title",
                                          IsIndeterminate   = true,
                                          IsProgressVisible = true,
                                          Progress          = 50
                                      };

            MessageBoxManager.Default.QuickSetButtons(messageBoxViewModel,
                                                      MessageButtonTypes.YesNoCancel,
                                                      isCloseButtonVisible: true);


            MessageBoxManager.Default.ShowMessageBox(messageBoxViewModel);
        }
    }


    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private double _opacity;

        public double Opacity
        {
            get => _opacity;
            set => SetField(ref _opacity, value);
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}