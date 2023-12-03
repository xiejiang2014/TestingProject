using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WpfMessagBox;

public class ButtonBehavior : INotifyPropertyChanged
{
    public ButtonBehavior()
    {
        if (Application.Current.TryFindResource("MessageBoxButtonStyle") is Style style)
        {
            Style = style;
        }
    }

    public bool ForceHidden { get; set; }

    public bool IsDefault   { get; set; }

    public object? ButtonContent { get; set; }

    public Action? ClickAction { get; set; }

    public bool CanExecute { get; set; } = true;

    public Style? Style { get; set; }

    #region PropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}