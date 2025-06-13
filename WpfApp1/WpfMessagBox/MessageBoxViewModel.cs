using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfMessageBox;

public class MessageBoxViewModel : INotifyPropertyChanged
{
    public static Thickness DefaultPadding { get; set; } = new(0);

    public bool IsMaskVisible { get; set; } = true;

    public Brush? MaskBrush { get; set; }

    public string? Title { get; set; }

    public string? Message        { get; set; }

    public string? WarningMessage { get; set; }

    public object? CustomizeContent { get; set; }

    public MessageBoxResults Result { get; set; } = MessageBoxResults.None;

    public MessageBoxViewModel()
    {
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

    public double MinHeight { get; set; } = 600;
    public double MinWidth  { get; set; } = 800;
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


    public ButtonBehavior OkButtonBehavior { get; set; } = new()
                                                           {
                                                               IsDefault = true
                                                           };

    public ButtonBehavior YesButtonBehavior { get; set; } = new();


    public ButtonBehavior NoButtonBehavior { get; set; } = new();


    public ButtonBehavior CancelButtonBehavior { get; set; } = new()
                                                               {
                                                                   IsCancel = true,
                                                               };


    #region Close

    public ButtonBehavior CloseButtonBehavior              { get; }      = new();
    public bool           CloseWhenMaskMouseLeftButtonDown { get; set; } = false;


    public void Close()
    {
        IsClosed = true;
    }

    /// <summary>
    /// 是否已关闭
    /// </summary>
    public bool IsClosed { get; private set; }

    #endregion

    #region 进度

    public bool IsProgressVisible { get; set; }

    public bool IsIndeterminate { get; set; }

    public double Progress { get; set; }

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

    #region PropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}