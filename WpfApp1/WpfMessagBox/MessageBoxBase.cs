using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using WpfMessageBox.ValueConverter;

namespace WpfMessageBox;

[TemplatePart(Name = "PART_ButtonClose")]
public class MessageBoxBase : Control
{
    protected ButtonBase? ButtonClose;

    protected MessageBoxViewModel? MessageBoxViewModel;


    public MessageBoxBase()
    {
        DataContextChanged += MessageBoxBase_DataContextChanged;
    }

    private void MessageBoxBase_DataContextChanged(object                             sender,
                                                   DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is MessageBoxViewModel messageBoxViewModel)
        {
            MessageBoxViewModel = messageBoxViewModel;
        }
    }


    public override void OnApplyTemplate()
    {
        if (GetTemplateChild("PART_ButtonClose") is ButtonBase buttonClose)
        {
            ButtonClose       =  buttonClose;
            ButtonClose.Click += (_, _) => MessageBoxViewModel?.CloseButtonBehavior.ClickAction?.Invoke();

            BindVisibilityForButton(ButtonClose, MessageBoxViewModel?.CloseButtonBehavior);

            ButtonClose.SetBinding(StyleProperty,
                                   new Binding()
                                   {
                                       Source = MessageBoxViewModel?.CloseButtonBehavior,
                                       Path   = new PropertyPath(nameof(ButtonBehavior.Style))
                                   });

            ButtonClose.SetBinding(ContentControl.ContentProperty,
                                   new Binding()
                                   {
                                       Source = MessageBoxViewModel?.CloseButtonBehavior,
                                       Path   = new PropertyPath(nameof(ButtonBehavior.ButtonContent))
                                   });


            ButtonClose.SetBinding(IsEnabledProperty,
                                   new Binding()
                                   {
                                       Source = MessageBoxViewModel?.CloseButtonBehavior,
                                       Path   = new PropertyPath(nameof(ButtonBehavior.CanExecute))
                                   });
        }

        base.OnApplyTemplate();
    }


    private static void BindVisibilityForButton(ButtonBase      button,
                                                ButtonBehavior? buttonBehavior)
    {
        var multiBinding = new MultiBinding();
        multiBinding.Bindings.Add(
            new Binding() //action 为空时隐藏  
            {
                Source    = buttonBehavior,
                Path      = new PropertyPath("ClickAction"),
                Converter = IsNullConverter.Default
            }
        );


        multiBinding.Bindings.Add(
            new Binding() //ForceHidden==true 时隐藏  
            {
                Source = buttonBehavior,
                Path   = new PropertyPath("ForceHidden"),
            }
        );

        multiBinding.Converter = AnyTrueToCollapsedConverter.Default;

        button.SetBinding(VisibilityProperty, multiBinding);
    }

    ///////////// <summary>
    ///////////// 有任意一个按钮是显示状态时, 按钮面板才显示否则按钮面板会隐藏
    ///////////// </summary>
    ///////////// <param name="buttons"></param>
    ///////////// <param name="buttonsPanel"></param>
    //////////private static void BindVisibilityForButtonsPanel(List<ButtonBase?> buttons, Panel buttonsPanel)
    //////////{
    //////////    var multiBinding = new MultiBinding();

    //////////    foreach (var buttonBase in buttons)
    //////////    {
    //////////        if (buttonBase is not null)
    //////////        {
    //////////            multiBinding.Bindings.Add(new Binding()
    //////////            {
    //////////                Source = buttonBase,
    //////////                Path   = new PropertyPath("Visibility")
    //////////            });
    //////////        }
    //////////    }

    //////////    if (!multiBinding.Bindings.Any())
    //////////    {
    //////////        return;
    //////////    }

    //////////    multiBinding.Converter = AnyVisibleToVisibleConverter.Default;

    //////////    buttonsPanel.SetBinding(VisibilityProperty, multiBinding);
    //////////}
}