using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MessageBox.Avalonia;

public class ButtonBehavior : INotifyPropertyChanged
{
    public bool ForceHidden { get; set; }

    public bool IsCancel { get; set; }

    public bool IsDefault { get; set; }

    public object? ButtonContent { get; set; }

    public Action? ClickAction { get; set; }

    public bool CanExecute { get; set; } = true;

    #region PropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}