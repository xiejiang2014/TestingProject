using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ShareDrawing.Tools.MessageBox
{
    public class ButtonBehavior : INotifyPropertyChanged
    {
        public object ButtonContent { get; set; }

        public Action ClickAction { get; set; }

        //public ICommand Command { get; set; }

        public bool CanExecute { get; set; } = true;

        public bool IsAccent { get; set; }




        #region PropertyChanged


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
