using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using PropertyChanged;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace AvaMessageBox;

[DoNotNotify]
public partial class MessageBoxManager : UserControl
{
    public static MessageBoxManager Default { get; } = new();

    /// <summary>
    /// 默认的弹窗背景遮罩色
    /// </summary>
    public SolidColorBrush DefaultBackground { get; set; } = new(Color.FromArgb(50, 0, 0, 0));

    public MessageBoxManager()
    {
        InitializeComponent();


        AddHandler(Button.ClickEvent, Buttons_OnClick);
        //var textBlock = new TextBlock();
        //var text      = textBlock.GetObservable(TextBlock.TextProperty);

        //this.GetObservable()
    }

    private void Buttons_OnClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Button { DataContext: ButtonBehavior { CanExecute: true } buttonBehavior })
        {
            buttonBehavior.ClickAction?.Invoke();
        }
    }


    private readonly ConcurrentDictionary<MessageBoxViewModel, MessageLayer>
        _messageBoxViewModelAndLayerDic = new();


    #region 完全自定义内容框

    /// <summary>
    /// 显示自定义内容对话框
    /// </summary>
    /// <param name="customizeContent">自定义内容对象</param>
    /// <param name="title">标题</param>
    /// <param name="messageButtonType">按钮类型</param>
    /// <param name="isCloseButtonVisible">是否显示关闭按钮</param>
    /// <returns></returns>
    public MessageBoxViewModel CreateCustomizeMessageBox(
        object             customizeContent,
        string             title                = "",
        MessageButtonTypes messageButtonType    = MessageButtonTypes.OkOnly,
        bool               isCloseButtonVisible = false)
    {
        var messageBoxViewModel = new MessageBoxViewModel
                                  {
                                      CustomizeContent = customizeContent,
                                      Title            = title,
                                      MessageBoxType   = MessageBoxTypes.Customize
                                  };

        return SetMessageBoxViewModelButtons(messageBoxViewModel, messageButtonType, isCloseButtonVisible);
    }

    #endregion 完全自定义内容框


    #region 文本消息框

    /// <summary>
    /// 显示自定义文本对话框
    /// </summary>
    /// <param name="message">自定义文本</param>
    /// <param name="title">标题</param>
    /// <param name="messageButtonType">按钮类型</param>
    /// <param name="isCloseButtonVisible">是否显示关闭按钮</param>
    /// <returns></returns>
    public MessageBoxViewModel CreateTextMessageBoxViewModel(
        string             message,
        string             title                = "",
        MessageButtonTypes messageButtonType    = MessageButtonTypes.OkOnly,
        bool               isCloseButtonVisible = false
    )
    {
        var messageBoxViewModel = new MessageBoxViewModel()
                                  {
                                      Message        = message,
                                      Title          = title,
                                      MessageBoxType = MessageBoxTypes.TextMessage
                                  };

        return SetMessageBoxViewModelButtons(messageBoxViewModel, messageButtonType, isCloseButtonVisible);
    }

    #endregion 文本消息框

    #region 显示等待框

    /// <summary>
    /// 显示一个正在等待的对话框,该对话框没有可交互元素,用户可通过取消按钮发送取消请求.
    /// </summary>
    /// <param name="message">要显示的文本</param>
    /// <param name="title">标题文本</param>
    /// <param name="isCancelButtonVisible">是否显示取消按钮</param>
    /// <returns></returns>
    public MessageBoxViewModel CreateWaitingMessageBox(string message               = "",
                                                       string title                 = "",
                                                       bool   isCancelButtonVisible = false
    )
    {
        var messageBoxViewModel = new MessageBoxViewModel()
                                  {
                                      Message        = message,
                                      Title          = title,
                                      MessageBoxType = MessageBoxTypes.Waiting
                                  };

        return SetMessageBoxViewModelButtons(messageBoxViewModel,
                                             isCancelButtonVisible
                                                 ? MessageButtonTypes.CancelOnly
                                                 : MessageButtonTypes.NoButton,
                                             false);
    }

    #endregion 显示等待框


    private MessageBoxViewModel SetMessageBoxViewModelButtons(MessageBoxViewModel messageBoxViewModel,
                                                              MessageButtonTypes  messageButtonType,
                                                              bool                isCloseButtonVisible)
    {
        var isOkButtonVisible     = false;
        var isYesButtonVisible    = false;
        var isNoButtonVisible     = false;
        var isCancelButtonVisible = false;

        switch (messageButtonType)
        {
            case MessageButtonTypes.NoButton:
                break;

            case MessageButtonTypes.OkOnly:
                isOkButtonVisible = true;
                break;

            case MessageButtonTypes.YesNo:
                isYesButtonVisible = true;
                isNoButtonVisible  = true;
                break;

            case MessageButtonTypes.YesNoCancel:
                isYesButtonVisible    = true;
                isNoButtonVisible     = true;
                isCancelButtonVisible = true;
                break;

            case MessageButtonTypes.OkCancel:
                isOkButtonVisible     = true;
                isCancelButtonVisible = true;
                break;

            case MessageButtonTypes.CancelOnly:
                isCancelButtonVisible = true;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(messageButtonType), messageButtonType, null);
        }


        //按下ok按钮时要执行的委托
        if (isOkButtonVisible)
        {
            messageBoxViewModel.ButtonBehaviors.Add(messageBoxViewModel.OkButtonBehavior);
            messageBoxViewModel.OkButtonBehavior.ClickAction = () =>
                                                               {
                                                                   //将对话框结果设为ok,并关闭对话框
                                                                   messageBoxViewModel.Result = MessageBoxResults.Ok;
                                                                   CloseMessageBox(messageBoxViewModel);
                                                               };
        }

        //同上
        if (isYesButtonVisible)
        {
            messageBoxViewModel.ButtonBehaviors.Add(messageBoxViewModel.YesButtonBehavior);
            messageBoxViewModel.YesButtonBehavior.ClickAction = () =>
                                                                {
                                                                    messageBoxViewModel.Result = MessageBoxResults.Yes;
                                                                    CloseMessageBox(messageBoxViewModel);
                                                                };
        }

        //同上
        if (isNoButtonVisible)
        {
            messageBoxViewModel.ButtonBehaviors.Add(messageBoxViewModel.NoButtonBehavior);
            messageBoxViewModel.NoButtonBehavior.ClickAction = () =>
                                                               {
                                                                   messageBoxViewModel.Result = MessageBoxResults.No;
                                                                   CloseMessageBox(messageBoxViewModel);
                                                               };
        }

        //同上
        if (isCancelButtonVisible)
        {
            messageBoxViewModel.ButtonBehaviors.Add(messageBoxViewModel.CancelButtonBehavior);
            messageBoxViewModel.CancelButtonBehavior.ClickAction = () =>
                                                                   {
                                                                       messageBoxViewModel.Result = MessageBoxResults.Cancel;
                                                                       CloseMessageBox(messageBoxViewModel);
                                                                   };
        }

        //同上
        if (isCloseButtonVisible)
        {
            messageBoxViewModel.CloseButtonBehavior.ClickAction = () =>
                                                                  {
                                                                      messageBoxViewModel.Result = MessageBoxResults.Close;
                                                                      CloseMessageBox(messageBoxViewModel);
                                                                  };
        }

        return messageBoxViewModel;
    }


    /// <summary>
    /// 显示一个消息框
    /// </summary>
    /// <param name="messageBoxViewModel"></param>
    public void ShowMessageBox(MessageBoxViewModel messageBoxViewModel)
    {
        if (messageBoxViewModel is null)
        {
            throw new ArgumentNullException(nameof(messageBoxViewModel));
        }

        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Invoke(() => ShowMessageBox(messageBoxViewModel));
            return;
        }

        //为 messageBoxViewModel 创建显示层并显示
        var newLayer = new MessageLayer { MessageBoxViewModel = messageBoxViewModel };

        if (messageBoxViewModel.MaskBrush is null) //在没有明确指定遮罩的画刷时,按照默认规则设置遮罩颜色.
        {
            if (messageBoxViewModel.IsMaskVisible)
            {
                var background = messageBoxViewModel.MessageBoxType switch
                                 {
                                     MessageBoxTypes.Waiting     => Application.Current?.FindResource("WaitingMessageBackground"),
                                     MessageBoxTypes.TextMessage => Application.Current?.FindResource("TextMessageBackground"),
                                     MessageBoxTypes.Customize   => Application.Current?.FindResource("CustomizeBackground"),
                                     _ => DefaultBackground
                                 } ?? DefaultBackground;

                if (background is IBrush brush)
                {
                    messageBoxViewModel.MaskBrush = brush;
                }
                else
                {
                    messageBoxViewModel.MaskBrush = DefaultBackground;
                }
            }
            else
            {
                messageBoxViewModel.MaskBrush = Brushes.Transparent;
            }
        }


        _messageBoxViewModelAndLayerDic.TryAdd(messageBoxViewModel, newLayer);
        LayersPanel.Children.Add(newLayer);
    }


    public void HideAllMessageBoxes()
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Invoke(HideAllMessageBoxes);
            return;
        }

        foreach (var keyValuePair in _messageBoxViewModelAndLayerDic)
        {
            keyValuePair.Value.IsVisible = false;
        }
    }

    /// <summary>
    /// 隐藏指定的消息框
    /// </summary>
    /// <param name="messageBoxViewModel"></param>
    public void HideMessageBox(MessageBoxViewModel messageBoxViewModel)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Invoke(() => HideMessageBox(messageBoxViewModel));
            return;
        }

        if (_messageBoxViewModelAndLayerDic.TryGetValue(messageBoxViewModel, out var layer))
        {
            layer.IsVisible = false;
        }
    }

    /// <summary>
    /// 将隐藏的消息框重新显示
    /// </summary>
    /// <param name="messageBoxViewModel"></param>
    public void DisplayMessageBox(MessageBoxViewModel messageBoxViewModel)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Invoke(() => DisplayMessageBox(messageBoxViewModel));
            return;
        }

        if (_messageBoxViewModelAndLayerDic.TryGetValue(messageBoxViewModel, out var layer))
        {
            layer.IsVisible = true;
        }
    }

    public void DisplayAllMessageBoxes()
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Invoke(DisplayAllMessageBoxes);
            return;
        }

        foreach (var keyValuePair in _messageBoxViewModelAndLayerDic)
        {
            keyValuePair.Value.IsVisible = true;
        }
    }
    
    /// <summary>
    /// 关闭指定的消息框
    /// </summary>
    /// <param name="messageBoxViewModel"></param>
    public void CloseMessageBox(MessageBoxViewModel messageBoxViewModel)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Invoke(() => CloseMessageBox(messageBoxViewModel));
            return;
        }

        //从显示容器中移除
        if (_messageBoxViewModelAndLayerDic.TryRemove(messageBoxViewModel, out var layer))
        {
            LayersPanel.Children.Remove(layer);
        }

        messageBoxViewModel.Close();
    }

    public MessageBoxViewModel? LastOrDefaultMessageBox()
    {
        return _messageBoxViewModelAndLayerDic.Any() ? _messageBoxViewModelAndLayerDic.Last().Key : null;
    }

}