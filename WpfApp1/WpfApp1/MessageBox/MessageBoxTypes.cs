namespace WpfApp1.MessageBox
{
    public enum MessageBoxTypes
    {
        /// <summary>
        /// 显示一个正在等待的对话框,该对话框没有可交互元素
        /// </summary>
        Waiting,

        /// <summary>
        /// 显示一个文本消息框,该对话框没有可交互元素
        /// </summary>
        TextMessage,

        /// <summary>
        /// 自定义内容的消息框.
        /// </summary>
        Customize,

        /// <summary>
        /// 自定义内容并附带按钮的消息框.
        /// </summary>
        CustomizeWithButton,
    }
}