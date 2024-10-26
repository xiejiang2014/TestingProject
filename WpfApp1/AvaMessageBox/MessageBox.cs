using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using PropertyChanged;

namespace AvaMessageBox;

[DoNotNotify]
[TemplatePart("PART_Title",                   typeof(TextBlock))]
[TemplatePart("PART_MessageTextBlock",        typeof(TextBlock))]
[TemplatePart("PART_ButtonsControl",          typeof(ItemsControl))]
[TemplatePart("PART_CustomizeContentControl", typeof(ContentControl))]
[TemplatePart("PART_ButtonClose",             typeof(Button))]
public class MessageBox : TemplatedControl
{
    //protected Button? ButtonClose;

    //protected MessageBoxViewModel? MessageBoxViewModel;

    public MessageBox()
    {
        //DataContextChanged += MessageBox_DataContextChanged;
    }

    //private void MessageBox_DataContextChanged(object? sender, System.EventArgs e)
    //{
    //    if (DataContext is MessageBoxViewModel messageBoxViewModel)
    //    {
    //        MessageBoxViewModel = messageBoxViewModel;
    //    }
    //}
}