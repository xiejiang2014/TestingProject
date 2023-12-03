using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfMessageBox.ValueConverter;

namespace WpfMessageBox;

/// <summary>
/// MessageBoxManager.xaml 的交互逻辑
/// </summary>
public partial class MessageBoxManager : INotifyPropertyChanged
{
    public static MessageBoxManager Default { get; } = new();


    /// <summary>
    /// 默认的弹窗背景遮罩色
    /// </summary>
    public SolidColorBrush DefaultBackground { get; set; } = new(Color.FromArgb(50, 0, 0, 0));

    public MessageBoxManager()
    {
        InitializeComponent();

        var boolToVisibilityConverter = new BoolToVisibilityConverter {UseHidden = true};

        //将自身的 Visibility 绑定到自身的 IsShowingAnyMessageBox 属性上
        var binding = new Binding()
        {
            Source    = this,
            Path      = new PropertyPath(nameof(IsAnyMessageBoxVisible)),
            Converter = boolToVisibilityConverter
        };
        SetBinding(VisibilityProperty, binding);

        AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Button_OnClick));
    }


    private void Button_OnClick(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is Button {DataContext: ButtonBehavior {CanExecute: true} buttonBehavior})
        {
            buttonBehavior.ClickAction?.Invoke();
        }
    }

    /// <summary>
    /// 当前是否显示了任何对话框
    /// </summary>
    public bool IsAnyMessageBoxVisible { get; private set; }


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

        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(() => ShowMessageBox(messageBoxViewModel));
            return;
        }

        //为 messageBoxViewModel 创建显示层并显示
        var newLayer = new MessageLayer {MessageBoxViewModel = messageBoxViewModel};

        if (messageBoxViewModel.MaskBrush is null) //在没有明确指定遮罩的画刷时,按照默认规则设置遮罩颜色.
        {
            if (messageBoxViewModel.IsMaskVisible)
            {
                var background = messageBoxViewModel.MessageBoxType switch
                                 {
                                     MessageBoxTypes.Waiting =>
                                         Application.Current?.TryFindResource("WaitingMessageBackground"),

                                     MessageBoxTypes.TextMessage =>
                                         Application.Current?.TryFindResource("TextMessageBackground"),

                                     MessageBoxTypes.Customize =>
                                         Application.Current?.TryFindResource("CustomizeBackground"),

                                     _ => DefaultBackground
                                 } ?? DefaultBackground;

                if (background is Brush brush)
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

        IsAnyMessageBoxVisible = true;
    }

    public void HideAllMessageBoxes()
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(HideAllMessageBoxes);
            return;
        }

        foreach (var keyValuePair in _messageBoxViewModelAndLayerDic)
        {
            keyValuePair.Value.Visibility = Visibility.Hidden;
        }
    }

    /// <summary>
    /// 隐藏指定的消息框
    /// </summary>
    /// <param name="messageBoxViewModel"></param>
    public void HideMessageBox(MessageBoxViewModel messageBoxViewModel)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(() => HideMessageBox(messageBoxViewModel));
            return;
        }

        if (_messageBoxViewModelAndLayerDic.TryGetValue(messageBoxViewModel, out var layer))
        {
            layer.Visibility = Visibility.Hidden;
        }
    }

    /// <summary>
    /// 将隐藏的消息框重新显示
    /// </summary>
    /// <param name="messageBoxViewModel"></param>
    public void DisplayMessageBox(MessageBoxViewModel messageBoxViewModel)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(() => DisplayMessageBox(messageBoxViewModel));
            return;
        }

        if (_messageBoxViewModelAndLayerDic.TryGetValue(messageBoxViewModel, out var layer))
        {
            layer.Visibility = Visibility.Visible;
        }
    }

    public void DisplayAllMessageBoxes()
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(DisplayAllMessageBoxes);
            return;
        }

        foreach (var keyValuePair in _messageBoxViewModelAndLayerDic)
        {
            keyValuePair.Value.Visibility = Visibility.Visible;
        }
    }


    public void CloseMessageBoxWidthDefaultClosingAnimation(MessageBoxViewModel messageBoxViewModel)
    {
        RunDefaultClosingAnimation(messageBoxViewModel,
                                   () => CloseMessageBox(messageBoxViewModel)
        );
    }

    /// <summary>
    /// 关闭指定的消息框
    /// </summary>
    /// <param name="messageBoxViewModel"></param>
    public void CloseMessageBox(MessageBoxViewModel messageBoxViewModel)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(() => CloseMessageBox(messageBoxViewModel));
            return;
        }

        //从显示容器中移除
        if (_messageBoxViewModelAndLayerDic.TryRemove(messageBoxViewModel, out var layer))
        {
            LayersPanel.Children.Remove(layer);

            IsAnyMessageBoxVisible = _messageBoxViewModelAndLayerDic.Any();
        }

        messageBoxViewModel.Close();
    }

    public MessageBoxViewModel? LastOrDefaultMessageBox()
    {
        return _messageBoxViewModelAndLayerDic.Any() ? _messageBoxViewModelAndLayerDic.Last().Key : null;
    }


    #region 动画

    /// <summary>
    /// 执行对话框的显示动画,通常在调用 ShowMessageBox 之后,立刻使用
    /// </summary>
    /// <param name="messageBoxViewModel"></param>
    /// <param name="animationCompletedAction">动画完成后的回调函数</param>
    public void RunDefaultShowingAnimation(MessageBoxViewModel messageBoxViewModel,
                                           Action?             animationCompletedAction = null
    )
    {
        RunDefaultShowingAnimation(messageBoxViewModel, -1, -1, animationCompletedAction);
    }

    /// <summary>
    /// 执行对话框的显示动画
    /// </summary>
    /// <param name="messageBoxViewModel">对话框的 ViewModel</param>
    /// <param name="startX">起始坐标x,默认-1,表示对话框显示区域的中心</param>
    /// <param name="startY">起始坐标y,默认-1,表示对话框显示区域的中心</param>
    /// <param name="animationCompletedAction">动画完成后的回调函数</param>
    public void RunDefaultShowingAnimation(MessageBoxViewModel messageBoxViewModel,
                                           int                 startX,
                                           int                 startY,
                                           Action?             animationCompletedAction = null
    )
    {
        if (!_messageBoxViewModelAndLayerDic.TryGetValue(messageBoxViewModel, out var layer))
        {
            return;
        }


        //创建缩放变形
        var scaleTransform = new ScaleTransform(0.01, 0.01);

        //将缩放变形应用到对象上
        layer.MessageLayerRootContentControl.RenderTransform = scaleTransform;

        //以起始点为中心进行缩放.如果没有指定起始点,那么默认从本MessageBoxManager的中心点
        var pointX = startX == -1 ? 0.5d : startX / ActualWidth;
        var pointY = startY == -1 ? 0.5d : startY / ActualHeight;

        layer.MessageLayerRootContentControl.RenderTransformOrigin = new Point(pointX, pointY);

        //创建背景笔刷
        var maskBackgroundBrush = new SolidColorBrush(Colors.Transparent);

        //设置初始背景
        layer.Mask.Fill = maskBackgroundBrush;

        try
        {
            //创建缩放动画
            RegisterName("AnimatedScaleTransform", scaleTransform); //为位缩放形注册名称
            var timeLines = CreateScalingTimeline("AnimatedScaleTransform", 1).ToList();

            //创建颜色变化动画
            RegisterName("MaskBackgroundBrush", maskBackgroundBrush);

            timeLines.AddRange(CreateColorChangingTimeline("MaskBackgroundBrush",
                                                           Colors.Transparent,
                                                           Color.FromArgb(99, 0, 0, 0)
                               )
            );

            //创建故事版,并将所有的动画放入故事版中
            var translationStoryboard = new Storyboard();
            timeLines.ForEach(translationStoryboard.Children.Add);

            translationStoryboard.Completed += (_,
                                                _
            ) =>
            {
                //todo 这里应该删除动画
                //control.RenderTransform = null;
                //control.Background = new SolidColorBrush(Color.FromArgb(99, 0, 0, 0));
                translationStoryboard.Stop();

                animationCompletedAction?.Invoke();
            };

            //播放动画
            translationStoryboard.Begin(this);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
        finally
        {
            //注销用到的对象名称
            UnregisterName("AnimatedScaleTransform");
            UnregisterName("MaskBackgroundBrush");
        }
    }

    public void RunDefaultClosingAnimation(MessageBoxViewModel messageBoxViewModel,
                                           Action?             animationCompletedAction = null
    )
    {
        RunDefaultClosingAnimation(messageBoxViewModel, -1, -1, animationCompletedAction);
    }


    public void RunDefaultClosingAnimation(MessageBoxViewModel messageBoxViewModel,
                                           int                 endX,
                                           int                 endY,
                                           Action?             animationCompletedAction = null
    )
    {
        if (!_messageBoxViewModelAndLayerDic.TryGetValue(messageBoxViewModel, out var layer))
        {
            return;
        }

        //创建缩放变形
        var scaleTransform = new ScaleTransform(1, 1);

        //将缩放变形应用到对象上
        layer.MessageLayerRootContentControl.RenderTransform = scaleTransform;

        //以起始点为中心进行缩放.如果没有指定起始点,那么默认从本MessageBoxManager的中心点
        var pointX = endX == -1 ? 0.5d : endX / ActualWidth;
        var pointY = endY == -1 ? 0.5d : endY / ActualHeight;
        layer.MessageLayerRootContentControl.RenderTransformOrigin = new Point(pointX, pointY);

        try
        {
            //创建缩放动画
            RegisterName("AnimatedScaleTransform", scaleTransform); //为缩放变形注册名称
            var timeLines = CreateScalingTimeline("AnimatedScaleTransform", 0.01).ToList();

            //创建颜色变化动画
            var maskBackgroundBrush = layer.Mask.Fill;
            RegisterName("MaskBackgroundBrush", maskBackgroundBrush);

            if (maskBackgroundBrush is SolidColorBrush solidColorBrush)
            {
                timeLines.AddRange(CreateColorChangingTimeline("MaskBackgroundBrush",
                                                               solidColorBrush.Color,
                                                               Colors.Transparent
                                   )
                );
            }


            //创建故事版,并将所有的动画放入故事版中
            var translationStoryboard = new Storyboard();
            timeLines.ForEach(translationStoryboard.Children.Add);

            translationStoryboard.Completed += (_,
                                                _
            ) =>
            {
                //todo 这里应该删除动画
                translationStoryboard.Stop();
                animationCompletedAction?.Invoke();
            };

            //播放动画
            translationStoryboard.Begin(this);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
        finally
        {
            //注销用到的对象名称
            UnregisterName("AnimatedScaleTransform");
            UnregisterName("MaskBackgroundBrush");
        }
    }


    /// <summary>
    /// 创建缩放移动画
    /// </summary>
    /// <param name="targetName">目标对象</param>
    /// <param name="targetValue">目标值</param>
    /// <returns></returns>
    private static IEnumerable<Timeline> CreateScalingTimeline(string targetName,
                                                               double targetValue
    )
    {
        //X 轴平缩放画  不管初始态如何,保证在结束时调整为 targetValue
        var scaleXDoubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames()
        {
            FillBehavior = FillBehavior.HoldEnd,
            KeyFrames = new DoubleKeyFrameCollection()
            {
                new EasingDoubleKeyFrame()
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100)),
                    Value   = targetValue,
                    EasingFunction = new CircleEase()
                    {
                        EasingMode = EasingMode.EaseIn
                    }
                }
            }
        };

        Storyboard.SetTargetName(scaleXDoubleAnimationUsingKeyFrames, targetName);
        Storyboard.SetTargetProperty(scaleXDoubleAnimationUsingKeyFrames,
                                     new PropertyPath(ScaleTransform.ScaleXProperty));
        yield return scaleXDoubleAnimationUsingKeyFrames;

        //Y 轴平缩放画  不管初始态如何,保证在结束时调整为 targetValue
        var scaleYDoubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames()
        {
            FillBehavior = FillBehavior.HoldEnd,
            KeyFrames = new DoubleKeyFrameCollection()
            {
                new EasingDoubleKeyFrame()
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100)),
                    Value   = targetValue,
                    EasingFunction = new CircleEase()
                    {
                        EasingMode = EasingMode.EaseIn
                    }
                }
            }
        };

        Storyboard.SetTargetName(scaleYDoubleAnimationUsingKeyFrames, targetName);
        Storyboard.SetTargetProperty(scaleYDoubleAnimationUsingKeyFrames,
                                     new PropertyPath(ScaleTransform.ScaleYProperty));
        yield return scaleYDoubleAnimationUsingKeyFrames;
    }

    /// <summary>
    /// 创建颜色转变动画对象
    /// </summary>
    /// <param name="targetName">目标名称</param>
    /// <param name="startingColor">起始颜色</param>
    /// <param name="targetColor">目标颜色</param>
    /// <returns></returns>
    private static IEnumerable<Timeline> CreateColorChangingTimeline(string targetName,
                                                                     Color? startingColor,
                                                                     Color  targetColor)
    {
        EasingColorKeyFrame easingColorKeyFrame;

        if (startingColor is null)
        {
            easingColorKeyFrame = new EasingColorKeyFrame()
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200)),
                Value   = targetColor,
            };
        }
        else
        {
            easingColorKeyFrame = new EasingColorKeyFrame(startingColor.Value)
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200)),
                Value   = targetColor,
            };
        }

        var colorAnimationUsingKeyFrames = new ColorAnimationUsingKeyFrames()
        {
            FillBehavior = FillBehavior.HoldEnd,
            KeyFrames = new ColorKeyFrameCollection()
            {
                easingColorKeyFrame
            }
        };

        Storyboard.SetTargetName(colorAnimationUsingKeyFrames, targetName);
        Storyboard.SetTargetProperty(colorAnimationUsingKeyFrames, new PropertyPath(SolidColorBrush.ColorProperty));

        yield return colorAnimationUsingKeyFrames;
    }

    #endregion 动画

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}