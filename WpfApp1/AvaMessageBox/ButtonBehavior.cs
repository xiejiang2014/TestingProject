using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Styling;

namespace AvaMessageBox;

public class ButtonBehavior : INotifyPropertyChanged
{
    public ButtonBehavior()
    {

    }

    public bool ForceHidden { get; set; }

    public bool IsDefault   { get; set; }

    public object? ButtonContent { get; set; }

    public Action? ClickAction { get; set; }

    public bool CanExecute { get; set; } = true;
    
    //todo 应该用 class 伪类替代
    public Style? Style { get; set; }

    #region PropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}