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
    /// Ĭ�ϵĵ�����������ɫ
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


    #region ��ȫ�Զ������ݿ�

    /// <summary>
    /// ��ʾ�Զ������ݶԻ���
    /// </summary>
    /// <param name="customizeContent">�Զ������ݶ���</param>
    /// <param name="title">����</param>
    /// <param name="messageButtonType">��ť����</param>
    /// <param name="isCloseButtonVisible">�Ƿ���ʾ�رհ�ť</param>
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

    #endregion ��ȫ�Զ������ݿ�


    #region �ı���Ϣ��

    /// <summary>
    /// ��ʾ�Զ����ı��Ի���
    /// </summary>
    /// <param name="message">�Զ����ı�</param>
    /// <param name="title">����</param>
    /// <param name="messageButtonType">��ť����</param>
    /// <param name="isCloseButtonVisible">�Ƿ���ʾ�رհ�ť</param>
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

    #endregion �ı���Ϣ��

    #region ��ʾ�ȴ���

    /// <summary>
    /// ��ʾһ�����ڵȴ��ĶԻ���,�öԻ���û�пɽ���Ԫ��,�û���ͨ��ȡ����ť����ȡ������.
    /// </summary>
    /// <param name="message">Ҫ��ʾ���ı�</param>
    /// <param name="title">�����ı�</param>
    /// <param name="isCancelButtonVisible">�Ƿ���ʾȡ����ť</param>
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

    #endregion ��ʾ�ȴ���


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


        //����ok��ťʱҪִ�е�ί��
        if (isOkButtonVisible)
        {
            messageBoxViewModel.ButtonBehaviors.Add(messageBoxViewModel.OkButtonBehavior);
            messageBoxViewModel.OkButtonBehavior.ClickAction = () =>
                                                               {
                                                                   //���Ի�������Ϊok,���رնԻ���
                                                                   messageBoxViewModel.Result = MessageBoxResults.Ok;
                                                                   CloseMessageBox(messageBoxViewModel);
                                                               };
        }

        //ͬ��
        if (isYesButtonVisible)
        {
            messageBoxViewModel.ButtonBehaviors.Add(messageBoxViewModel.YesButtonBehavior);
            messageBoxViewModel.YesButtonBehavior.ClickAction = () =>
                                                                {
                                                                    messageBoxViewModel.Result = MessageBoxResults.Yes;
                                                                    CloseMessageBox(messageBoxViewModel);
                                                                };
        }

        //ͬ��
        if (isNoButtonVisible)
        {
            messageBoxViewModel.ButtonBehaviors.Add(messageBoxViewModel.NoButtonBehavior);
            messageBoxViewModel.NoButtonBehavior.ClickAction = () =>
                                                               {
                                                                   messageBoxViewModel.Result = MessageBoxResults.No;
                                                                   CloseMessageBox(messageBoxViewModel);
                                                               };
        }

        //ͬ��
        if (isCancelButtonVisible)
        {
            messageBoxViewModel.ButtonBehaviors.Add(messageBoxViewModel.CancelButtonBehavior);
            messageBoxViewModel.CancelButtonBehavior.ClickAction = () =>
                                                                   {
                                                                       messageBoxViewModel.Result = MessageBoxResults.Cancel;
                                                                       CloseMessageBox(messageBoxViewModel);
                                                                   };
        }

        //ͬ��
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
    /// ��ʾһ����Ϣ��
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

        //Ϊ messageBoxViewModel ������ʾ�㲢��ʾ
        var newLayer = new MessageLayer { MessageBoxViewModel = messageBoxViewModel };

        if (messageBoxViewModel.MaskBrush is null) //��û����ȷָ�����ֵĻ�ˢʱ,����Ĭ�Ϲ�������������ɫ.
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
    /// ����ָ������Ϣ��
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
    /// �����ص���Ϣ��������ʾ
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
    /// �ر�ָ������Ϣ��
    /// </summary>
    /// <param name="messageBoxViewModel"></param>
    public void CloseMessageBox(MessageBoxViewModel messageBoxViewModel)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Invoke(() => CloseMessageBox(messageBoxViewModel));
            return;
        }

        //����ʾ�������Ƴ�
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