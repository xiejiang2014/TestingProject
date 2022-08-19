using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp1.MessageBox
{
    public class ButtonBehavior : INotifyPropertyChanged
    {
        public object? ButtonContent { get; set; }

        public Action? ClickAction { get; set; }

        public bool CanExecute { get; set; } = true;

        public bool IsAccent { get; set; }


        #region PropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}