using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using AvaMessageBox;

namespace AvaMessageBoxDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
            //var userContent = new UserContent2();
            //var messageBoxViewModel =
            //    MessageBoxManager.Default.CreateCustomizeMessageBox(userContent,
            //                                                        "自定义标题",
            //                                                        MessageButtonTypes.NoButton,
            //                                                        true);

            //messageBoxViewModel.HorizontalContentAlignment = HorizontalAlignment.Center;
            //messageBoxViewModel.VerticalContentAlignment   = VerticalAlignment.Center;
            //messageBoxViewModel.MaskBrush                  = Brushes.Transparent;

            //messageBoxViewModel.ButtonBehaviors.Add(new ButtonBehavior()
            //                                        {
            //                                            ButtonContent = "自定义按钮",
            //                                            ClickAction = () =>
            //                                                          {
            //                                                              MessageBoxManager.Default.CloseMessageBoxWidthDefaultClosingAnimation(messageBoxViewModel);
            //                                                          },
            //                                            Style = Application.Current.FindResource("ExtButtonStyle") as Style
            //                                        });

            //messageBoxViewModel.CloseButtonBehavior.ClickAction = () =>
            //                                                      {
            //                                                          MessageBoxManager.Default.CloseMessageBoxWidthDefaultClosingAnimation(messageBoxViewModel);
            //                                                      };


            //if (Application.Current.TryFindResource("CustomizeMessageBoxStyleXXX") is Style style)
            //{
            //    messageBoxViewModel.Style = style;
            //}

            //messageBoxViewModel.HorizontalAlignment = HorizontalAlignment.Right;
            //messageBoxViewModel.VerticalAlignment   = VerticalAlignment.Stretch;

            //MessageBoxManager.Default.ShowMessageBox(messageBoxViewModel);
            //MessageBoxManager.Default.RunDefaultShowingAnimation(messageBoxViewModel);
        }

        private void ButtonBase3_OnClick(object? sender, RoutedEventArgs e)
        {
            var messageBoxViewModel = new MessageBoxViewModel()
                                      {
                                          Message = "标题",
                                          Title   = "文本内容",
                                      };

            MessageBoxManager.Default.QuickSetButtons(messageBoxViewModel,
                                                      MessageButtonTypes.YesNoCancel,
                                                      isCloseButtonVisible: true);


            MessageBoxManager.Default.ShowMessageBox(messageBoxViewModel);
        }
    }
}