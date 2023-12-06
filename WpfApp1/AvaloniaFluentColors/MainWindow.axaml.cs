using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Media;

namespace AvaloniaFluentColors
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ColorsList.ItemsSource = new List<FluentColor>()
                                     {
                                         new()
                                         {
                                             ColorName      = "SystemAltHighColor", LightColorBrush = SolidColorBrush.Parse("#FFFFFFFF"),
                                             DarkColorBrush = SolidColorBrush.Parse("#FF000000")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemAltLowColor", LightColorBrush = SolidColorBrush.Parse("#33FFFFFF"),
                                             DarkColorBrush = SolidColorBrush.Parse("#33000000")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemAltMediumColor", LightColorBrush = SolidColorBrush.Parse("#99FFFFFF"),
                                             DarkColorBrush = SolidColorBrush.Parse("#99000000")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemAltMediumHighColor", LightColorBrush = SolidColorBrush.Parse("#CCFFFFFF"),
                                             DarkColorBrush = SolidColorBrush.Parse("#CC000000")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemAltMediumLowColor", LightColorBrush = SolidColorBrush.Parse("#66FFFFFF"),
                                             DarkColorBrush = SolidColorBrush.Parse("#66000000")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemBaseHighColor", LightColorBrush = SolidColorBrush.Parse("#FF000000"),
                                             DarkColorBrush = SolidColorBrush.Parse("#FFFFFFFF")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemBaseLowColor", LightColorBrush = SolidColorBrush.Parse("#33000000"),
                                             DarkColorBrush = SolidColorBrush.Parse("#33FFFFFF")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemBaseMediumColor", LightColorBrush = SolidColorBrush.Parse("#99000000"),
                                             DarkColorBrush = SolidColorBrush.Parse("#99FFFFFF")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemBaseMediumHighColor", LightColorBrush = SolidColorBrush.Parse("#CC000000"),
                                             DarkColorBrush = SolidColorBrush.Parse("#CCFFFFFF")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemBaseMediumLowColor", LightColorBrush = SolidColorBrush.Parse("#66000000"),
                                             DarkColorBrush = SolidColorBrush.Parse("#66FFFFFF")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemChromeAltLowColor", LightColorBrush = SolidColorBrush.Parse("#FF171717"),
                                             DarkColorBrush = SolidColorBrush.Parse("#FFF2F2F2")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemChromeBlackHighColor", LightColorBrush = SolidColorBrush.Parse("#FF000000"),
                                             DarkColorBrush = SolidColorBrush.Parse("#FF000000")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemChromeBlackLowColor", LightColorBrush = SolidColorBrush.Parse("#33000000"),
                                             DarkColorBrush = SolidColorBrush.Parse("#33000000")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemChromeBlackMediumLowColor", LightColorBrush = SolidColorBrush.Parse("#66000000"),
                                             DarkColorBrush = SolidColorBrush.Parse("#66000000")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemChromeBlackMediumColor", LightColorBrush = SolidColorBrush.Parse("#CC000000"),
                                             DarkColorBrush = SolidColorBrush.Parse("#CC000000")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemChromeDisabledHighColor", LightColorBrush = SolidColorBrush.Parse("#FFCCCCCC"),
                                             DarkColorBrush = SolidColorBrush.Parse("#FF333333")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemChromeDisabledLowColor", LightColorBrush = SolidColorBrush.Parse("#FF7A7A7A"),
                                             DarkColorBrush = SolidColorBrush.Parse("#FF858585")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemChromeHighColor", LightColorBrush = SolidColorBrush.Parse("#FFCCCCCC"),
                                             DarkColorBrush = SolidColorBrush.Parse("#FF767676")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemChromeLowColor", LightColorBrush = SolidColorBrush.Parse("#FFF2F2F2"),
                                             DarkColorBrush = SolidColorBrush.Parse("#FF171717")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemChromeMediumColor", LightColorBrush = SolidColorBrush.Parse("#FFE6E6E6"),
                                             DarkColorBrush = SolidColorBrush.Parse("#FF1F1F1F")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemChromeMediumLowColor", LightColorBrush = SolidColorBrush.Parse("#FFF2F2F2"),
                                             DarkColorBrush = SolidColorBrush.Parse("#FF2B2B2B")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemChromeWhiteColor", LightColorBrush = SolidColorBrush.Parse("#FFFFFFFF"),
                                             DarkColorBrush = SolidColorBrush.Parse("#FFFFFFFF")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemChromeGrayColor", LightColorBrush = SolidColorBrush.Parse("#FF767676"),
                                             DarkColorBrush = SolidColorBrush.Parse("#FF767676")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemListLowColor", LightColorBrush = SolidColorBrush.Parse("#19000000"),
                                             DarkColorBrush = SolidColorBrush.Parse("#19FFFFFF")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemListMediumColor", LightColorBrush = SolidColorBrush.Parse("#33000000"),
                                             DarkColorBrush = SolidColorBrush.Parse("#33FFFFFF")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemErrorTextColor", LightColorBrush = SolidColorBrush.Parse("#C50500"),
                                             DarkColorBrush = SolidColorBrush.Parse("#FFF000")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemRegionColor", LightColorBrush = SolidColorBrush.Parse("#FFFFFFFF"),
                                             DarkColorBrush = SolidColorBrush.Parse("#FF000000")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemRevealListLowColor", LightColorBrush = SolidColorBrush.Parse("#17000000"),
                                             DarkColorBrush = SolidColorBrush.Parse("#18FFFFFF")
                                         },
                                         new()
                                         {
                                             ColorName      = "SystemRevealListMediumColor", LightColorBrush = SolidColorBrush.Parse("#2E000000"),
                                             DarkColorBrush = SolidColorBrush.Parse("#30FFFFFF")
                                         },
                                     };
        }
    }
}