using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using AvaMessageBox.ValueConverter;
using PropertyChanged;

namespace AvaMessageBox;

[DoNotNotify]
[TemplatePart("PART_ButtonClose", typeof(Button))]
public class MessageBoxBase : TemplatedControl
{
    protected Button? ButtonClose;

    protected MessageBoxViewModel? MessageBoxViewModel;

    public MessageBoxBase()
    {
        DataContextChanged += MessageBoxBase_DataContextChanged;
    }

    private void MessageBoxBase_DataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is MessageBoxViewModel messageBoxViewModel)
        {
            MessageBoxViewModel = messageBoxViewModel;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (e.NameScope.Find("PART_ButtonClose") is Button buttonClose)
        {
            ButtonClose       =  buttonClose;
            ButtonClose.Click += (_, _) => MessageBoxViewModel?.CloseButtonBehavior.ClickAction?.Invoke();

            BindVisibilityForButton(ButtonClose, MessageBoxViewModel?.CloseButtonBehavior);

            ButtonClose.Bind(ContentControl.ContentProperty,   new Binding(nameof(ButtonBehavior.ButtonContent)) { Source = MessageBoxViewModel?.CloseButtonBehavior });
            ButtonClose.Bind(ContentControl.IsEnabledProperty, new Binding(nameof(ButtonBehavior.CanExecute)) { Source    = MessageBoxViewModel?.CloseButtonBehavior });
        }


        base.OnApplyTemplate(e);
    }


    private static void BindVisibilityForButton(Button          button,
                                                ButtonBehavior? buttonBehavior)
    {
        var multiBinding = new MultiBinding();
        multiBinding.Bindings.Add(
                                  new Binding() //action 为空时隐藏  
                                  {
                                      Source = buttonBehavior,
                                      Path   = nameof(ButtonBehavior.ClickAction),
                                      Converter = ObjectConverters.IsNull
                                  }
                                 );


        multiBinding.Bindings.Add(
                                  new Binding() //ForceHidden==true 时隐藏  
                                  {
                                      Source = buttonBehavior,
                                      Path   = nameof(ButtonBehavior.ForceHidden),
                                  }
                                 );

        multiBinding.Converter = AllFalseConverter.Default;

        button.Bind(IsVisibleProperty, multiBinding);
    }
}