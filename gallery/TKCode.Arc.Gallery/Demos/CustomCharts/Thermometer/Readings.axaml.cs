using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TKCode.Arc.Gallery.Demos;

public partial class ThermometerReadingsDemo : UserControl
{
    public ThermometerReadingsDemo()
    {
        InitializeComponent();
        Temperature.PropertyChanged += (_, change) =>
        {
            if (change.Property == Slider.ValueProperty)
            {
                Live.Value = Temperature.Value;
            }
        };
    }

    private void OnLowClick(object? sender, RoutedEventArgs e) => Temperature.Value = -8;
    private void OnNormalClick(object? sender, RoutedEventArgs e) => Temperature.Value = 36.5;
    private void OnHighClick(object? sender, RoutedEventArgs e) => Temperature.Value = 86;
}
