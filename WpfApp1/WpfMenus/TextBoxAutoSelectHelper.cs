﻿using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WpfMenus
{
    //https: //blog.csdn.net/jiuzaizuotian2014/article/details/81868842

    /// <summary>
    /// 当 TextBoxBase获得焦点的时候，自动全部选择文字。附加属性为SelectAllWhenGotFocus，类型为bool.
    /// </summary>
    public class TextBoxAutoSelectHelper
    {
        public static readonly DependencyProperty SelectAllWhenGotFocusProperty =
            DependencyProperty.RegisterAttached("SelectAllWhenGotFocus",
                                                typeof(bool), typeof(TextBoxAutoSelectHelper),
                                                new FrameworkPropertyMetadata(false,
                                                                              OnSelectAllWhenGotFocusChanged));

        public static bool GetSelectAllWhenGotFocus(TextBoxBase d)
        {
            return (bool) d.GetValue(SelectAllWhenGotFocusProperty);
        }

        public static void SetSelectAllWhenGotFocus(TextBoxBase d, bool value)
        {
            d.SetValue(SelectAllWhenGotFocusProperty, value);
        }

        private static void OnSelectAllWhenGotFocusChanged(DependencyObject                   dependency,
                                                           DependencyPropertyChangedEventArgs e)
        {
            if (dependency is TextBoxBase tBox)
            {
                var isSelectedAllWhenGotFocus = (bool) e.NewValue;
                if (isSelectedAllWhenGotFocus)
                {
                    tBox.PreviewMouseDown += TextBoxPreviewMouseDown;
                    tBox.GotFocus         += TextBoxOnGotFocus;
                    tBox.LostFocus        += TextBoxOnLostFocus;
                }
                else
                {
                    tBox.PreviewMouseDown -= TextBoxPreviewMouseDown;
                    tBox.GotFocus         -= TextBoxOnGotFocus;
                    tBox.LostFocus        -= TextBoxOnLostFocus;
                }
            }
        }

        private static async void TextBoxOnGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBoxBase tBox)
            {
                await Task.Yield();
                tBox.SelectAll();
                tBox.PreviewMouseDown -= TextBoxPreviewMouseDown;
            }
        }

        private static void TextBoxPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBoxBase tBox)
            {
                tBox.Focus();
                e.Handled = true;
            }
        }

        private static void TextBoxOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBoxBase tBox)
            {
                tBox.PreviewMouseDown += TextBoxPreviewMouseDown;
            }
        }
    }
}