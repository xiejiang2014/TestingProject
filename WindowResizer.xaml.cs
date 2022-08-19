using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ShareDrawing.CommonUI
{
    /// <summary>
    /// WindowResizer.xaml 的交互逻辑
    /// </summary>
    public partial class WindowResizer
    {
        public WindowResizer()
        {
            InitializeComponent();

            Loaded += WindowResizer_Loaded;
        }

        private Window _window;

        private void WindowResizer_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyObject reference = this;

            while (true)
            {
                var parent = VisualTreeHelper.GetParent(reference);

                if (parent is Window window)
                {
                    _window = window;
                    break;
                }

                if (parent is null)
                {
                    break;
                }

                reference = parent;
            }

            AddResizerLeft(LeftThumb);
            AddResizerLeftBottom(LeftBottomThumb);
            AddResizerRightBottom(RightBottomThumb);
            AddResizerRight(RightThumb);
            AddResizerTop(TopThumb);
            AddResizerBottom(BottomThumb);
        }



        public bool IsResizing
        {
            get => (bool) GetValue(IsResizingProperty);
            private set => SetValue(IsResizingKey, value);
        }

        private static readonly DependencyPropertyKey IsResizingKey
            = DependencyProperty.RegisterReadOnly(
                "IsResizing",
                typeof(bool),
                typeof(WindowResizer),
                new PropertyMetadata(default(bool), IsResizingPropertyChanged));

        private static void IsResizingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WindowResizer windowResizer)
            {

            }
        }


        public static readonly DependencyProperty IsResizingProperty
            = IsResizingKey.DependencyProperty;


        private bool _isResizingRight;
        private bool _isResizingLeft;
        private bool _isResizingTop;
        private bool _isResizingBottom;

        private readonly Dictionary<UIElement, short> _leftElements = new();
        private readonly Dictionary<UIElement, short> _rightElements = new();
        private readonly Dictionary<UIElement, short> _topElements = new();
        private readonly Dictionary<UIElement, short> _bottomElements = new();

        private PointApi _mouseDownPoint;


        /// <summary>
        /// 调整开始时的窗口尺寸
        /// </summary>
        private Size _resizingInitialSize;

        /// <summary>
        /// 调整开始时的窗口位置
        /// </summary>
        private Point _resizingInitialLocation;


        #region 添加用于调整的元素

        public void AddResizerRight(UIElement element)
        {
            ConnectMouseHandlers(element);
            _rightElements.Add(element, 0);
        }

        public void AddResizerLeft(UIElement element)
        {
            ConnectMouseHandlers(element);
            _leftElements.Add(element, 0);
        }

        public void AddResizerTop(UIElement element)
        {
            ConnectMouseHandlers(element);
            _topElements.Add(element, 0);
        }

        public void AddResizerBottom(UIElement element)
        {
            ConnectMouseHandlers(element);
            _bottomElements.Add(element, 0);
        }

        public void AddResizerRightBottom(UIElement element)
        {
            ConnectMouseHandlers(element);
            _rightElements.Add(element, 0);
            _bottomElements.Add(element, 0);
        }

        public void AddResizerLeftBottom(UIElement element)
        {
            ConnectMouseHandlers(element);
            _leftElements.Add(element, 0);
            _bottomElements.Add(element, 0);
        }

        public void AddResizerRightTop(UIElement element)
        {
            ConnectMouseHandlers(element);
            _rightElements.Add(element, 0);
            _topElements.Add(element, 0);
        }

        public void AddResizerLeftTop(UIElement element)
        {
            ConnectMouseHandlers(element);
            _leftElements.Add(element, 0);
            _topElements.Add(element, 0);
        }

        private void ConnectMouseHandlers(UIElement uiElement)
        {
            uiElement.MouseLeftButtonDown += Element_MouseLeftButtonDown;
            uiElement.MouseLeftButtonUp += Element_MouseLeftButtonUp;
            uiElement.MouseMove += UiElement_MouseMove;
        }

        #endregion


        #region 元素的鼠标按下和抬起

        private void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement uiElement)
            {
                uiElement.ReleaseMouseCapture();
            }

            IsResizing = false;
        }

        private void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not UIElement uiElement || _window is null)
            {
                return;
            }


            uiElement.CaptureMouse();

            GetCursorPos(out _mouseDownPoint);

            _resizingInitialSize = new Size(_window.ActualWidth, _window.ActualHeight);
            _resizingInitialLocation = new Point(_window.Left, _window.Top);

            //识别当前是在向哪个方向进行调整
            _isResizingLeft = _leftElements.ContainsKey(uiElement);
            _isResizingRight = _rightElements.ContainsKey(uiElement);
            _isResizingTop = _topElements.ContainsKey(uiElement);
            _isResizingBottom = _bottomElements.ContainsKey(uiElement);

            IsResizing = _isResizingLeft || _isResizingRight || _isResizingTop || _isResizingBottom;
        }


        private void UiElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (_window is null)
            {
                return;
            }

            if (Mouse.LeftButton == MouseButtonState.Pressed &&
                (_isResizingBottom || _isResizingLeft || _isResizingRight || _isResizingTop))
            {
                GetCursorPos(out var currentMousePoint);

                var targetWidth = _window.ActualWidth;
                var targetHeight = _window.ActualHeight;
                var targetLeft = _window.Left;
                var targetTop = _window.Top;

                if (_isResizingRight)
                {
                    targetWidth = _resizingInitialSize.Width - (_mouseDownPoint.X - currentMousePoint.X);
                    if (targetWidth <= _window.MinWidth)
                    {
                        targetWidth = _window.MinWidth;
                    }
                }

                if (_isResizingBottom)
                {
                    targetHeight = _resizingInitialSize.Height - (_mouseDownPoint.Y - currentMousePoint.Y);
                    if (targetHeight <= _window.MinHeight)
                    {
                        targetHeight = _window.MinHeight;
                    }
                }

                if (_isResizingLeft)
                {
                    targetWidth = _resizingInitialSize.Width + (_mouseDownPoint.X - currentMousePoint.X);
                    if (targetWidth <= _window.MinWidth)
                    {
                        targetWidth = _window.MinWidth;
                        targetLeft = _resizingInitialLocation.X + _resizingInitialSize.Width - _window.MinWidth;
                    }
                    else
                    {
                        targetLeft = _resizingInitialLocation.X - (_mouseDownPoint.X - currentMousePoint.X);
                    }
                }

                if (_isResizingTop)
                {
                    targetHeight = _resizingInitialSize.Height + (_mouseDownPoint.Y - currentMousePoint.Y);
                    if (targetHeight <= _window.MinHeight)
                    {
                        targetHeight = _window.MinHeight;
                        targetTop = _resizingInitialLocation.Y + _resizingInitialSize.Height - _window.MinHeight;
                    }
                    else
                    {
                        targetTop = _resizingInitialLocation.Y - (_mouseDownPoint.Y - currentMousePoint.Y);
                    }
                }


                _window.Width = targetWidth;


                _window.Height = targetHeight;


                _window.Left = targetLeft;
                _window.Top = targetTop;
            }
        }

        #endregion


        #region external call

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out PointApi lpPoint);

        private struct PointApi
        {
            public int X;
            public int Y;
        }

        #endregion
    }
}