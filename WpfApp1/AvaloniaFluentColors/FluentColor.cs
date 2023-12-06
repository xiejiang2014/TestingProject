using Avalonia.Media;

namespace AvaloniaFluentColors;

public class FluentColor
{
    public string ColorName { get; set; }

    public IBrush LightColorBrush { get; set; }
    public IBrush DarkColorBrush { get; set; }
}