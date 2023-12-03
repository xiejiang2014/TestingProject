using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfMessagBox;

public interface ICustomizeContentViewModel : INotifyPropertyChanged
{
    Action? MessageBoxRequestToClose { get; set; }

    Action? MessageBoxRequestToOk { get; set; }
}

public class MessageBoxViewModel : INotifyPropertyChanged
{
    public static Thickness DefaultPadding { get; set; } = new(0);

    public MessageBoxTypes MessageBoxType { get; set; }

    public bool IsMaskVisible { get; set; } = true;

    public Brush? MaskBrush { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public object? CustomizeContent { get; set; }

    private ICustomizeContentViewModel? _customizeContentViewModel;

    public ICustomizeContentViewModel? CustomizeContentViewModel
    {
        get => _customizeContentViewModel;
        set
        {
            if (_customizeContentViewModel is not null)
            {
                _customizeContentViewModel.MessageBoxRequestToClose -= MessageBoxRequestToClose;
                _customizeContentViewModel.MessageBoxRequestToOk    -= MessageBoxRequestToOk;
            }

            _customizeContentViewModel = value;

            if (_customizeContentViewModel is not null)
            {
                _customizeContentViewModel.MessageBoxRequestToClose += MessageBoxRequestToClose;
                _customizeContentViewModel.MessageBoxRequestToOk    += MessageBoxRequestToOk;
            }
        }
    }

    private void MessageBoxRequestToOk()
    {
        Result = MessageBoxResults.Ok;
        if (OkButtonBehavior.CanExecute)
        {
            OkButtonBehavior.ClickAction?.Invoke();
        }
    }

    private void MessageBoxRequestToClose()
    {
        Result = MessageBoxResults.Close;
        if (CloseButtonBehavior.CanExecute)
        {
            CloseButtonBehavior.ClickAction?.Invoke();
        }
    }

    public MessageBoxResults Result { get; set; } = MessageBoxResults.None;

    public MessageBoxViewModel()
    {
        OkButtonBehavior.ButtonContent     = OkButtonDefaultContent;
        YesButtonBehavior.ButtonContent    = YesButtonDefaultContent;
        NoButtonBehavior.ButtonContent     = NoButtonDefaultContent;
        CancelButtonBehavior.ButtonContent = CancelButtonDefaultContent;
        CloseButtonBehavior.ButtonContent  = CloseButtonDefaultContent;

        Padding = DefaultPadding;
    }

    #region 样式

    public Style? Style { get; set; }

    #endregion

    #region 布局

    public Thickness           Padding                    { get; set; }
    public Thickness           Margin                     { get; set; } = new(0, 0, 0, 0);
    public HorizontalAlignment HorizontalAlignment        { get; set; } = HorizontalAlignment.Center;
    public VerticalAlignment   VerticalAlignment          { get; set; } = VerticalAlignment.Center;
    public HorizontalAlignment HorizontalContentAlignment { get; set; } = HorizontalAlignment.Center;
    public VerticalAlignment   VerticalContentAlignment   { get; set; } = VerticalAlignment.Center;

    public double MaxHeight { get; set; } = double.MaxValue;
    public double MaxWidth  { get; set; } = double.MaxValue;


    /// <summary>
    /// 是否使用 ScrollViewer
    /// </summary>
    public bool IsScrollViewerVisible =>
        HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled || //两个滚动条有任何一个可用,则要使用ScrollViewer
        VerticalScrollBarVisibility   != ScrollBarVisibility.Disabled;

    public ScrollBarVisibility HorizontalScrollBarVisibility { get; set; } = ScrollBarVisibility.Auto;
    public ScrollBarVisibility VerticalScrollBarVisibility   { get; set; } = ScrollBarVisibility.Auto;

    #endregion


    #region 按钮集合

    public ObservableCollection<ButtonBehavior> ButtonBehaviors { get; set; } = new();

    #endregion


    #region Ok

    public        ButtonBehavior OkButtonBehavior       { get; set; } = new();
    public static object         OkButtonDefaultContent { get; set; } = "确定";

    #endregion

    #region Yes

    public        ButtonBehavior YesButtonBehavior       { get; set; } = new();
    public static object         YesButtonDefaultContent { get; set; } = "是";

    #endregion

    #region No

    public        ButtonBehavior NoButtonBehavior       { get; set; } = new();
    public static object         NoButtonDefaultContent { get; set; } = "否";

    #endregion

    #region Cancel

    public ButtonBehavior CancelButtonBehavior { get; set; } = new();

    public static object CancelButtonDefaultContent { get; set; } = "取消";

    #endregion


    #region Close

    public        ButtonBehavior CloseButtonBehavior              { get; }      = new();
    public static object         CloseButtonDefaultContent        { get; set; } = "X";
    public        bool           CloseWhenMaskMouseLeftButtonDown { get; set; } = false;
    

    public void Close()
    {
        IsClosed = true;

        if (_customizeContentViewModel is not null)
        {
            _customizeContentViewModel.MessageBoxRequestToClose -= MessageBoxRequestToClose;
            _customizeContentViewModel.MessageBoxRequestToOk    -= MessageBoxRequestToOk;
        }
    }

    /// <summary>
    /// 是否已关闭
    /// </summary>
    public bool IsClosed { get; private set; }

    #endregion

    public Task WaitUntilClosed()
    {
        return Task.Run(() =>
        {
            while (!IsClosed)
            {
                Thread.Sleep(1);
            }
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}