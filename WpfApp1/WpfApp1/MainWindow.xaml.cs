using System.Threading.Tasks;
using System.Windows;
using WpfMessageBox;

namespace WpfApp1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        ContentControl.Content = MessageBoxManager.Default;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var messageBoxViewModel =
            MessageBoxManager.Default.CreateTextMessageBoxViewModel("文本消息",
                                                                "自定义标题",
                                                                MessageButtonTypes.YesNoCancel,
                                                                isCloseButtonVisible: true);
        MessageBoxManager.Default.ShowMessageBox(messageBoxViewModel);
    }

    private async void ButtonBase2_OnClick(object sender, RoutedEventArgs e)
    {
        var userContent = new UserContent();
        var messageBoxViewModel =
            MessageBoxManager.Default.CreateCustomizeMessageBox(userContent,
                                                                "自定义标题",
                                                                MessageButtonTypes.NoButton,
                                                                true);

        MessageBoxManager.Default.ShowMessageBox(messageBoxViewModel);

        await Task.Delay(6000);

        messageBoxViewModel.ButtonBehaviors.Add(new ButtonBehavior()
        {
            ButtonContent = "自定义按钮",
            ClickAction   = () => { MessageBoxManager.Default.CloseMessageBox(messageBoxViewModel); },
            Style         = Application.Current.FindResource("ExtButtonStyle") as Style
        });
    }

    private void ButtonBase3_OnClick(object sender, RoutedEventArgs e)
    {
        var messageBoxViewModel = MessageBoxManager.Default.CreateWaitingMessageBox("文本内容", "标题", true);
        MessageBoxManager.Default.ShowMessageBox(messageBoxViewModel);
    }
}