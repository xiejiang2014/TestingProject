using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ShareDrawing.Tools.MessageBox.ValueConverter;

namespace ShareDrawing.Tools.MessageBox
{
    /// <summary>
    /// MessageBoxManager.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBoxManager : INotifyPropertyChanged
    {
        public static MessageBoxManager Default { get; } = new();


        public static MessageBoxManager Right { get; } = new();



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
                Source = this,
                Path = new PropertyPath(nameof(IsShowingAnyMessageBox)),
                Converter = boolToVisibilityConverter
            };
            SetBinding(VisibilityProperty, binding);
        }

        /// <summary>
        /// 当前是否显示了任何对话框
        /// </summary>
        public bool IsShowingAnyMessageBox { get; private set; }


        private readonly ConcurrentDictionary<MessageBoxViewModel, MessageLayer>
            _messageBoxViewModelAndLayerDic = new();

        #region 完全自定义内容框

        /// <summary>
        /// 显示自定义内容对话框
        /// </summary>
        /// <param name="customizeContent">自定义内容对象</param>
        /// <returns></returns>
        public MessageBoxViewModel ShowCustomizeMessageBox(FrameworkElement customizeContent)
        {
            return ShowCustomizeMessageBox(customizeContent, string.Empty);
        }


        public MessageBoxViewModel ShowCustomizeMessageBox(FrameworkElement customizeContent, string title)
        {
            var messageBoxViewModel = new MessageBoxViewModel()
            {
                CustomizeContent = customizeContent,
                MessageBoxType = MessageBoxTypes.Customize,
                Title = title,
            };

            //默认的关闭行为
            messageBoxViewModel.CloseButtonBehavior.ClickAction = () => CloseMessageBox(messageBoxViewModel);
            
            ShowMessageBox(messageBoxViewModel);

            return messageBoxViewModel;
        }

        #endregion 完全自定义内容框

        #region 自定义内容+按钮

        //public Task<MessageBoxResults> ShowCustomizeWithButtonMessageBox(
        //    Control customizeContent)
        //{
        //    return ShowCustomizeWithButtonMessageBox(customizeContent, string.Empty, MessageButtonTypes.OkOnly);
        //}

        //public Task<MessageBoxResults> ShowCustomizeWithButtonMessageBox(
        //    Control            customizeContent,
        //    MessageButtonTypes messageButtonType)
        //{
        //    return ShowCustomizeWithButtonMessageBox(customizeContent, string.Empty, messageButtonType);
        //}


        //public Task<MessageBoxResults> ShowCustomizeWithButtonMessageBox(
        //    Control            customizeContent,
        //    string             title,
        //    MessageButtonTypes messageButtonType = MessageButtonTypes.OkOnly)
        //{
        //    var isShowOkButton     = false;
        //    var isShowYesButton    = false;
        //    var isShowNoButton     = false;
        //    var isShowCancelButton = false;

        //    switch (messageButtonType)
        //    {
        //        case MessageButtonTypes.NoButton:
        //            break;
        //        case MessageButtonTypes.OkOnly:
        //            isShowOkButton = true;
        //            break;
        //        case MessageButtonTypes.YesNo:
        //            isShowYesButton = true;
        //            isShowNoButton  = true;
        //            break;
        //        case MessageButtonTypes.YesNoCancel:
        //            isShowYesButton    = true;
        //            isShowNoButton     = true;
        //            isShowCancelButton = true;
        //            break;
        //        case MessageButtonTypes.OkCancel:
        //            isShowOkButton     = true;
        //            isShowCancelButton = true;
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(messageButtonType), messageButtonType, null);
        //    }

        //    return ShowCustomizeWithButtonMessageBox(customizeContent,
        //                                             title,
        //                                             isShowOkButton,
        //                                             isShowYesButton,
        //                                             isShowNoButton,
        //                                             isShowCancelButton);
        //}

        /// <summary>
        /// 显示自定义内容并带有按钮对话框
        /// </summary>
        /// <param name="customizeContent">自定义内容对象</param>
        /// <returns></returns>
        public MessageBoxViewModel ShowCustomizeWithButtonMessageBox(FrameworkElement customizeContent)
        {
            return ShowCustomizeWithButtonMessageBox(customizeContent, string.Empty);
        }

        /// <summary>
        /// 显示自定义内容并带有按钮对话框
        /// </summary>
        /// <param name="customizeContent">自定义内容对象</param>
        /// <param name="title"></param>
        /// <returns></returns>
        public MessageBoxViewModel ShowCustomizeWithButtonMessageBox(
            FrameworkElement customizeContent,
            string title
        )
        {
            var messageBoxViewModel = new MessageBoxViewModel()
            {
                CustomizeContent = customizeContent,
                Title = title,
                MessageBoxType = MessageBoxTypes.CustomizeWithButton
            };

            //默认的关闭行为
            messageBoxViewModel.CloseButtonBehavior.ClickAction = () => CloseMessageBox(messageBoxViewModel);

            //显示对话框
            ShowMessageBox(messageBoxViewModel);

            return messageBoxViewModel;
        }

        #endregion

        #region 文本消息框

        /// <summary>
        /// 显示文本消息附带按钮的对话框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <returns></returns>
        public Task<MessageBoxResults> ShowTextMessageBox(string message)
        {
            return ShowTextMessageBox(message, string.Empty, MessageButtonTypes.OkOnly);
        }


        /// <summary>
        /// 显示文本消息附带按钮的对话框
        /// </summary>
        /// <param name="message">消息文本</param>
        /// <param name="messageButtonType">按钮类型</param>
        /// <returns></returns>
        public Task<MessageBoxResults> ShowTextMessageBox(string message,
            MessageButtonTypes messageButtonType
        )
        {
            return ShowTextMessageBox(message, string.Empty, messageButtonType);
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
            MessageButtonTypes messageButtonType = MessageButtonTypes.OkOnly
        )
        {
            var messageBoxViewModel = CreateTextMessageBoxViewModel(message, title, messageButtonType);


            return await ShowTextMessageBox(messageBoxViewModel);
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
            var messageBoxViewModel = CreateTextMessageBoxViewModel(
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


            return await ShowTextMessageBox(messageBoxViewModel);
        }

        public async Task<MessageBoxResults> ShowTextMessageBox(MessageBoxViewModel messageBoxViewModel)
        {
            //显示对话框
            ShowMessageBox(messageBoxViewModel);

            //等待对话框关闭
            await messageBoxViewModel.WaitMessageBoxClose();

            //返回用户选择的结果
            return messageBoxViewModel.Result;
        }

        public MessageBoxViewModel CreateTextMessageBoxViewModel(
            string message,
            string title = "",
            MessageButtonTypes messageButtonType = MessageButtonTypes.OkOnly,
            bool isShowCloseButton = false
        )
        {
            var isShowOkButton = false;
            var isShowYesButton = false;
            var isShowNoButton = false;
            var isShowCancelButton = false;

            switch (messageButtonType)
            {
                case MessageButtonTypes.NoButton:
                    break;
                case MessageButtonTypes.OkOnly:
                    isShowOkButton = true;
                    break;
                case MessageButtonTypes.YesNo:
                    isShowYesButton = true;
                    isShowNoButton = true;
                    break;
                case MessageButtonTypes.YesNoCancel:
                    isShowYesButton = true;
                    isShowNoButton = true;
                    isShowCancelButton = true;
                    break;
                case MessageButtonTypes.OkCancel:
                    isShowOkButton = true;
                    isShowCancelButton = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageButtonType), messageButtonType, null);
            }

            var messageBoxViewModel = CreateTextMessageBoxViewModel(
                message,
                title,
                isShowOkButton,
                isShowYesButton,
                isShowNoButton,
                isShowCancelButton,
                isShowCloseButton);
            return messageBoxViewModel;
        }

        public MessageBoxViewModel CreateTextMessageBoxViewModel(
            string message,
            string title,
            bool isShowOkButton,
            bool isShowYesButton,
            bool isShowNoButton,
            bool isShowCancelButton,
            bool isShowCloseButton,
            object okButtonContent = null,
            object yesButtonContent = null,
            object noButtonContent = null,
            object cancelButtonContent = null)
        {
            var messageBoxViewModel = new MessageBoxViewModel()
            {
                Message = message,
                Title = title,
                MessageBoxType = MessageBoxTypes.TextMessage
            };

            if (okButtonContent is not null)
            {
                messageBoxViewModel.OkButtonBehavior.ButtonContent = okButtonContent;
            }

            if (yesButtonContent is not null)
            {
                messageBoxViewModel.YesButtonBehavior.ButtonContent = yesButtonContent;
            }

            if (noButtonContent is not null)
            {
                messageBoxViewModel.NoButtonBehavior.ButtonContent = noButtonContent;
            }

            if (cancelButtonContent is not null)
            {
                messageBoxViewModel.CancelButtonBehavior.ButtonContent = cancelButtonContent;
            }


            //按下ok按钮时要执行的委托
            if (isShowOkButton)
            {
     


                messageBoxViewModel.OkButtonBehavior.ClickAction = () =>
                {
                    //将对话框结果设为ok,并关闭对话框
                    messageBoxViewModel.Result = MessageBoxResults.Ok;
                    CloseMessageBox(messageBoxViewModel);
                };
            }

            //同上
            if (isShowYesButton)
            {

                messageBoxViewModel.YesButtonBehavior.ClickAction = () =>
                {
                    messageBoxViewModel.Result = MessageBoxResults.Yes;
                    CloseMessageBox(messageBoxViewModel);
                };
            }

            //同上
            if (isShowNoButton)
            {
                messageBoxViewModel.NoButtonBehavior.ClickAction = () =>
                {
                    messageBoxViewModel.Result = MessageBoxResults.No;
                    CloseMessageBox(messageBoxViewModel);
                };
            }

            //同上
            if (isShowCancelButton)
            {
                messageBoxViewModel.CancelButtonBehavior.ClickAction = () =>
                {
                    messageBoxViewModel.Result = MessageBoxResults.Cancel;
                    CloseMessageBox(messageBoxViewModel);
                };
            }

            //同上
            if (isShowCloseButton)
            {
                messageBoxViewModel.CloseButtonBehavior.ClickAction = () =>
                {
                    messageBoxViewModel.Result = MessageBoxResults.Close;
                    CloseMessageBox(messageBoxViewModel);
                };
            }

            return messageBoxViewModel;
        }

        #endregion 文本消息框

        #region 显示等待框

        /// <summary>
        /// 显示一个正在等待的对话框,该对话框没有可交互元素,用户可通过取消按钮发送取消请求.
        /// </summary>
        /// <param name="message">要显示的文本</param>
        /// <param name="title">标题文本</param>
        /// <param name="isShowCancelButton">是否显示取消按钮</param>
        /// <returns></returns>
        public MessageBoxViewModel ShowWaitingMessageBox(string message,
            string title = "",
            bool isShowCancelButton = false
        )
        {
            var messageBoxViewModel = new MessageBoxViewModel()
            {
                Message = message,
                Title = title,
                MessageBoxType = MessageBoxTypes.Waiting
            };

            if (isShowCancelButton)
            {
                messageBoxViewModel.CancelButtonBehavior.ClickAction = () =>
                {
                    //当用点击了取消按钮时,将窗口的结果标记为已取消,并关闭窗口
                    messageBoxViewModel.IsCanceled = true;
                    CloseMessageBox(messageBoxViewModel);
                };
            }

            ShowMessageBox(messageBoxViewModel);
            return messageBoxViewModel;
        }

        #endregion 显示等待框

        /// <summary>
        /// 显示一个消息框
        /// </summary>
        /// <param name="messageBoxViewModel"></param>
        private void ShowMessageBox(MessageBoxViewModel messageBoxViewModel)
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

            newLayer.Background = messageBoxViewModel.MessageBoxType switch
            {
                MessageBoxTypes.Waiting => (System.Windows.Application.Current.TryFindResource("WaitingMessageBackground") as Brush),
                MessageBoxTypes.TextMessage => (System.Windows.Application.Current.TryFindResource("TextMessageBackground") as Brush),
                MessageBoxTypes.Customize => (System.Windows.Application.Current.TryFindResource("CustomizeBackground") as Brush),
                MessageBoxTypes.CustomizeWithButton => (System.Windows.Application.Current.TryFindResource(
                    "CustomizeWithButtonBackground") as Brush),
                _ => DefaultBackground
            } ?? DefaultBackground;


            _messageBoxViewModelAndLayerDic.TryAdd(messageBoxViewModel, newLayer);
            LayersPanel.Children.Add(newLayer);

            IsShowingAnyMessageBox = true;
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

                IsShowingAnyMessageBox = _messageBoxViewModelAndLayerDic.Any();
            }

            messageBoxViewModel.IsClosed = true;
        }

        public MessageBoxViewModel LastOrDefaultMessageBox()
        {
            return _messageBoxViewModelAndLayerDic.Any() ? _messageBoxViewModelAndLayerDic.Last().Key : null;
        }


        #region 动画

        /// <summary>
        /// 执行对话框的显示动画
        /// </summary>
        /// <param name="control">对话框的view</param>
        /// <param name="animationCompletedAction">动画完成后的回调函数</param>
        public void RunDefaultShownAnimation(Control control,
            Action animationCompletedAction = null
        )
        {
            RunDefaultShownAnimation(control, -1, -1, animationCompletedAction);
        }

        /// <summary>
        /// 执行对话框的显示动画
        /// </summary>
        /// <param name="control">对话框的view</param>
        /// <param name="startX">起始坐标x,默认-1,表示对话框显示区域的中心</param>
        /// <param name="startY">起始坐标y,默认-1,表示对话框显示区域的中心</param>
        /// <param name="animationCompletedAction">动画完成后的回调函数</param>
        public void RunDefaultShownAnimation(Control control,
            int startX,
            int startY,
            Action animationCompletedAction = null
        )
        {
            //创建缩放变形
            var scaleTransform = new ScaleTransform(0.01, 0.01);

            //将缩放变形应用到对象上
            control.RenderTransform = scaleTransform;

            //以起始点为中心进行缩放.如果没有指定起始点,那么默认从本MessageBoxManager的中心点
            var pointX = startX == -1 ? 0.5d : startX / ActualWidth;
            var pointY = startY == -1 ? 0.5d : startY / ActualHeight;

            control.RenderTransformOrigin = new Point(pointX, pointY);

            //创建背景笔刷
            var maskBackgroundBrush = new SolidColorBrush(Colors.Transparent);
            //设置初始背景
            Background = maskBackgroundBrush;

            try
            {
                //创建缩放动画
                RegisterName("AnimatedScaleTransform", scaleTransform); //为位缩放形注册名称
                var timeLines = CreateScaleTimeline("AnimatedScaleTransform", 1).ToList();

                //创建颜色变化动画
                RegisterName("MaskBackgroundBrush", maskBackgroundBrush);
                timeLines.AddRange(CreateColorChangeTimeline("MaskBackgroundBrush", Color.FromArgb(99, 0, 0, 0)));

                //创建故事版,并将所有的动画放入故事版中
                var translationStoryboard = new Storyboard();
                timeLines.ForEach(translationStoryboard.Children.Add);

                translationStoryboard.Completed += (s,
                    e
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

        public void RunDefaultCloseAnimation(Control control,
            Action animationCompletedAction = null
        )
        {
            RunDefaultCloseAnimation(control, -1, -1, animationCompletedAction);
        }


        public void RunDefaultCloseAnimation(Control control,
            int endX,
            int endY,
            Action animationCompletedAction = null
        )
        {
            //创建缩放变形
            var scaleTransform = new ScaleTransform(1, 1);

            //将缩放变形应用到对象上
            control.RenderTransform = scaleTransform;

            //以起始点为中心进行缩放.如果没有指定起始点,那么默认从本MessageBoxManager的中心点
            var pointX = endX == -1 ? 0.5d : endX / ActualWidth;
            var pointY = endY == -1 ? 0.5d : endY / ActualHeight;
            control.RenderTransformOrigin = new Point(pointX, pointY);

            try
            {
                //创建缩放动画
                RegisterName("AnimatedScaleTransform", scaleTransform); //为位缩放形注册名称
                var timeLines = CreateScaleTimeline("AnimatedScaleTransform", 0.01).ToList();

                //创建颜色变化动画
                RegisterName("MaskBackgroundBrush", Background);
                timeLines.AddRange(CreateColorChangeTimeline("MaskBackgroundBrush", Color.FromArgb(1, 0, 0, 0)));

                //创建故事版,并将所有的动画放入故事版中
                var translationStoryboard = new Storyboard();
                timeLines.ForEach(translationStoryboard.Children.Add);

                translationStoryboard.Completed += (s,
                    e
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
        private static IEnumerable<Timeline> CreateScaleTimeline(string targetName,
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
                        KeyTime =
                            KeyTime.FromTimeSpan(TimeSpan
                                .FromMilliseconds(500)),
                        Value = targetValue,
                        EasingFunction = new CircleEase()
                        {
                            EasingMode = EasingMode.EaseOut
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
                        KeyTime =
                            KeyTime.FromTimeSpan(TimeSpan
                                .FromMilliseconds(500)),
                        Value = targetValue,
                        EasingFunction = new CircleEase()
                        {
                            EasingMode = EasingMode.EaseOut
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
        /// <param name="targetColor">目标颜色</param>
        /// <returns></returns>
        private static IEnumerable<Timeline> CreateColorChangeTimeline(string targetName,
            Color targetColor
        )
        {
            var colorAnimationUsingKeyFrames = new ColorAnimationUsingKeyFrames()
            {
                FillBehavior = FillBehavior.HoldEnd,
                KeyFrames = new ColorKeyFrameCollection()
                {
                    new EasingColorKeyFrame()
                    {
                        KeyTime =
                            KeyTime.FromTimeSpan(TimeSpan
                                .FromMilliseconds(500)),
                        Value = targetColor,
                        EasingFunction = new CircleEase()
                        {
                            EasingMode = EasingMode.EaseOut
                        }
                    }
                }
            };

            Storyboard.SetTargetName(colorAnimationUsingKeyFrames, targetName);
            Storyboard.SetTargetProperty(colorAnimationUsingKeyFrames, new PropertyPath(SolidColorBrush.ColorProperty));

            yield return colorAnimationUsingKeyFrames;
        }

        #endregion 动画

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}