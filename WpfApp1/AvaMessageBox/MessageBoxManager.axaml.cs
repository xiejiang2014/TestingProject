using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
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

    public static object OkButtonDefaultContent { get; set; } = "确定";

    public static object YesButtonDefaultContent { get; set; } = "是";

    public static object NoButtonDefaultContent { get; set; } = "否";

    public static object CancelButtonDefaultContent { get; set; } = "取消";

    public static object CloseButtonDefaultContent { get; set; } = "X";


    /// <summary>
    /// 默认的弹窗背景遮罩色
    /// </summary>
    public SolidColorBrush DefaultBackground { get; set; } = new(Color.FromArgb(50, 0, 0, 0));

    public MessageBoxManager()
    {
        InitializeComponent();

        //这个已经在 xaml 中实现了
        //this.Bind(UserControl.IsVisibleProperty, new Binding(path: nameof(IsAnyMessageBoxVisible))
        //                                         {
        //                                             Source = this
        //                                         });

        AddHandler(Button.ClickEvent, Buttons_OnClick);
    }

    private void Buttons_OnClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Button { DataContext: ButtonBehavior { CanExecute: true } buttonBehavior })
        {
            buttonBehavior.ClickAction?.Invoke();
        }
    }


    /// <summary>
    /// 当前是否显示了任何对话框
    /// </summary>
    public bool IsAnyMessageBoxVisible { get; private set; }


    private readonly ConcurrentDictionary<MessageBoxViewModel, MessageLayer> _messageBoxViewModelAndLayerDic = new();

    /// <summary>
    /// 快速设置按钮为 MessageButtonTypes 中提供的几种模式之一.
    /// 弹窗关闭后可从 MessageBoxViewModel.Result 判断按钮点击结果.
    /// </summary>
    /// <param name="messageBoxViewModel"></param>
    /// <param name="messageButtonType"></param>
    /// <param name="isCloseButtonVisible"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void QuickSetButtons(MessageBoxViewModel messageBoxViewModel,
                                MessageButtonTypes  messageButtonType,
                                bool                isCloseButtonVisible = false)
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
            messageBoxViewModel.OkButtonBehavior.ButtonContent = OkButtonDefaultContent;
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
            messageBoxViewModel.YesButtonBehavior.ButtonContent = YesButtonDefaultContent;
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
            messageBoxViewModel.NoButtonBehavior.ButtonContent = NoButtonDefaultContent;
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
            messageBoxViewModel.CancelButtonBehavior.ButtonContent = CancelButtonDefaultContent;
            messageBoxViewModel.CancelButtonBehavior.ClickAction = () =>
                                                                   {
                                                                       messageBoxViewModel.Result = MessageBoxResults.Cancel;
                                                                       CloseMessageBox(messageBoxViewModel);
                                                                   };
        }

        //同上
        if (isCloseButtonVisible)
        {
            messageBoxViewModel.CloseButtonBehavior.ButtonContent = CloseButtonDefaultContent;
            messageBoxViewModel.CloseButtonBehavior.ClickAction = () =>
                                                                  {
                                                                      messageBoxViewModel.Result = MessageBoxResults.Close;
                                                                      CloseMessageBox(messageBoxViewModel);
                                                                  };
        }
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
                messageBoxViewModel.MaskBrush ??= DefaultBackground;
            }
            else
            {
                messageBoxViewModel.MaskBrush = Brushes.Transparent;
            }
        }

        _messageBoxViewModelAndLayerDic.TryAdd(messageBoxViewModel, newLayer);
        LayersPanel.Children.Add(newLayer);

        IsAnyMessageBoxVisible = true;
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
        return _messageBoxViewModelAndLayerDic.Any()
            ? _messageBoxViewModelAndLayerDic.Last().Key
            : null;
    }

    //todo 动画
}