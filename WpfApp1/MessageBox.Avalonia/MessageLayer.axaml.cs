using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PropertyChanged;

namespace MessageBox.Avalonia;

[DoNotNotify]
public partial class MessageLayer : UserControl
{
    public MessageLayer()
    {
        InitializeComponent();
    }

    private MessageBoxViewModel? _messageBoxViewModel;

    public MessageBoxViewModel? MessageBoxViewModel
    {
        get => _messageBoxViewModel;
        set
        {
            _messageBoxViewModel = value;
            DataContext          = value;
        }
    }

    private void Mask_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (MessageBoxViewModel is null)
        {
            return;
        }

        if (MessageBoxViewModel.CloseWhenMaskMouseLeftButtonDown &&
            MessageBoxViewModel.CloseButtonBehavior.CanExecute)
        {
            MessageBoxViewModel.CloseButtonBehavior.ClickAction?.Invoke();
        }
    }

    private void MessageLayerRootContentControl_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = true;
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        var focusManager = TopLevel.GetTopLevel(this)?.FocusManager;

        var focused = focusManager?.GetFocusedElement();

        if (focused != null)
        {
            var next = KeyboardNavigationHandler.GetNext(focused, NavigationDirection.Next);
            next?.Focus();
        }
    }
}