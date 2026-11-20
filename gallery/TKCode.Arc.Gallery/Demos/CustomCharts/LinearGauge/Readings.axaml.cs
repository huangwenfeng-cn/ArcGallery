using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TKCode.Arc.Gallery.Demos;

// 阅读顺序：InitializeComponent 创建字段 → 滑块模拟采样 → 按钮恢复/改变质量。
// 这里只演示 UI 数据源，没有采集协议或计时器。两个订阅对象均属于本演示。
public partial class LinearGaugeReadingsDemo : UserControl
{
    public static readonly StyledProperty<MeasurementQuality> SampleQualityProperty =
        AvaloniaProperty.Register<LinearGaugeReadingsDemo, MeasurementQuality>(nameof(SampleQuality));
    public MeasurementQuality SampleQuality
    {
        get => GetValue(SampleQualityProperty);
        set => SetValue(SampleQualityProperty, value);
    }
    public LinearGaugeReadingsDemo()
    {
        InitializeComponent();
        Reading.PropertyChanged += (_, change) =>
        {
            if (change.Property == Slider.ValueProperty)
            {
                Force.Value = Reading.Value;
                SampleQuality = MeasurementQuality.Good;
            }
        };
    }
    private void OnReadingClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string text } &&
            double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            // 重复点击同值时滑块不通知，因此仍显式恢复质量。
            // 始终先更新数值，再恢复 Good，避免恢复时闪现断线前的旧数值。
            Reading.Value = value;
            Force.Value = value;
            SampleQuality = MeasurementQuality.Good;
        }
    }
    private void OnQualityClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: MeasurementQuality quality })
        {
            SampleQuality = quality; // 只更新绑定源，保存读数不清零。
        }
    }
}
