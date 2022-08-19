using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ShareDrawing.Tools.MessageBox
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow
    {
        public MessageWindow()
        {
            InitializeComponent();

            MessageBoxManager.DefaultBackground = Brushes.Transparent;
        }


        public void CloseMessageBox(MessageBoxViewModel messageBoxViewModel)
        {
            MessageBoxManager.CloseMessageBox(messageBoxViewModel);
        }

        #region 自定义内容

        public MessageBoxViewModel ShowCustomizeMessageBox(
            FrameworkElement customizeContent,
            int width,
            int height,
            string title)
        {
            var messageBoxViewModel = MessageBoxManager.ShowCustomizeMessageBox(customizeContent, title);
            _ = messageBoxViewModel.WaitMessageBoxClose().ContinueWith(t => Dispatcher.Invoke(Close));

            Width = width;
            Height = height;
            Left = SystemParameters.WorkArea.Width - width;
            Top = SystemParameters.WorkArea.Height - height;

            Show();

            return messageBoxViewModel;
        }

        #endregion


        #region 文本框

        /// <summary>
        /// 显示文本消息附带按钮的对话框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Task<MessageBoxResults> ShowTextMessageBox(string message, int width, int height)
        {
            return ShowTextMessageBox(message, width, height, MessageButtonTypes.OkOnly);
        }


        /// <summary>
        /// 显示文本消息附带按钮的对话框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="height"></param>
        /// <param name="messageButtonType">按钮类型</param>
        /// <param name="width"></param>
        /// <returns></returns>
        public Task<MessageBoxResults> ShowTextMessageBox(
            string message,
            int width,
            int height,
            MessageButtonTypes messageButtonType
        )
        {
            return ShowTextMessageBox(message, "", width, height, messageButtonType);
        }

        /// <summary>
        /// 显示文本消息附带按钮的对话框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="title">标题文本</param>
        /// <param name="messageButtonType">按钮类型</param>
        /// <returns></returns>
        public async Task<MessageBoxResults> ShowTextMessageBox(
            string message,
            string title,
            int width,
            int height,
            MessageButtonTypes messageButtonType
        )
        {
            var task = MessageBoxManager.ShowTextMessageBox
            (
                message,
                title,
                messageButtonType
            );

            _ = task.ContinueWith(t => this.Dispatcher.Invoke(Close));

            Width = width;
            Height = height;
            Left = SystemParameters.WorkArea.Width - width;
            Top = SystemParameters.WorkArea.Height - height;

            Show();

            return await task;
        }


        public async Task<MessageBoxResults> ShowTextMessageBox(
            MessageBoxViewModel messageBoxViewModel,
            int width,
            int height)
        {
            //显示对话框
            var task = MessageBoxManager.ShowTextMessageBox(messageBoxViewModel);

            _ = task.ContinueWith(t => this.Dispatcher.Invoke(Close));

            Width = width;
            Height = height;
            Left = SystemParameters.WorkArea.Width - width;
            Top = SystemParameters.WorkArea.Height - height;

            Show();

            return await task;
        }

        /// <summary>
        /// 显示文本消息附带按钮的对话框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="title">标题</param>
        /// <param name="isShowOkButton">是否显示"确定"按钮,默认true</param>
        /// <param name="isShowYesButton">是否显示"是"按钮,默认false</param>
        /// <param name="isShowNoButton">是否显示"否"按钮,默认false</param>
        /// <param name="isShowCancelButton">是否显示"取消"按钮,默认false</param>
        /// <param name="isShowCloseButton"></param>
        /// <param name="okButtonContent"></param>
        /// <param name="yesButtonContent"></param>
        /// <param name="noButtonContent"></param>
        /// <param name="cancelButtonContent"></param>
        /// <returns></returns>
        public async Task<MessageBoxResults> ShowTextMessageBox(
            string message,
            string title,
            bool isShowOkButton = true,
            bool isShowYesButton = false,
            bool isShowNoButton = false,
            bool isShowCancelButton = false,
            bool isShowCloseButton = false,
            object okButtonContent = null,
            object yesButtonContent = null,
            object noButtonContent = null,
            object cancelButtonContent = null
        )
        {
            var task = MessageBoxManager.ShowTextMessageBox
            (
                message,
                title,
                isShowOkButton,
                isShowYesButton,
                isShowNoButton,
                isShowCancelButton,
                isShowCloseButton,
                okButtonContent,
                yesButtonContent,
                noButtonContent,
                cancelButtonContent
            );

            _ = task.ContinueWith(t => this.Dispatcher.Invoke(Close));
            ShowDialog();
            return await task;
        }

        #endregion
    }
}