using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TKCode.Arc.Gallery.Demos;

public partial class ThermometerReadingsDemo : UserControl
{
    // 这是 Gallery 演示，不是设备采集程序。它用按钮和滑块模拟应用收到的数据。
    // SampleQuality 是能发出属性变化通知的数据源，XAML 把它绑定到 Live.Quality。
    // 实际应用可换成 ViewModel 的通知属性，控件本身无需知道网关或数据库的类型。
    public static readonly StyledProperty<MeasurementQuality> SampleQualityProperty =
        AvaloniaProperty.Register<ThermometerReadingsDemo, MeasurementQuality>(nameof(SampleQuality));

    public MeasurementQuality SampleQuality
    {
        get => GetValue(SampleQualityProperty);
        set => SetValue(SampleQualityProperty, value);
    }

    public ThermometerReadingsDemo()
    {
        // 先加载 XAML，才会创建名字为 Live/Temperature 的控件字段。
        InitializeComponent();
        // 监听本演示的滑块。PropertyChanged 会报告所有属性变化，
        // 所以只处理 ValueProperty，避免样式或布局变化被误当成新采样。
        Temperature.PropertyChanged += (_, change) =>
        {
            if (change.Property == Slider.ValueProperty)
            {
                // 按“先数值，后有效质量”的顺序模拟有效样本；恢复画面直接使用新数值。
                Live.Value = Temperature.Value;
                SampleQuality = MeasurementQuality.Good;
            }
        };
    }

    private void SetReading(double value)
    {
        // 同值按钮也要恢复：滑块已经是 36.5 时再次设置 36.5，不会触发变化事件，
        // 所以这里仍显式更新 Live.Value 和 SampleQuality，不完全依赖滑块回调。
        Temperature.Value = value;
        Live.Value = value;
        SampleQuality = MeasurementQuality.Good;
    }

    private void OnLowClick(object? sender, RoutedEventArgs e) => SetReading(-8);
    private void OnNormalClick(object? sender, RoutedEventArgs e) => SetReading(36.5);
    private void OnHighClick(object? sender, RoutedEventArgs e) => SetReading(86);

    private void OnQualityClick(object? sender, RoutedEventArgs e)
    {
        // sender 是被点击的控件。模式匹配确认它是 Button，且 Tag 保存质量枚举，
        // 同时取出枚举值 quality；Tag 由 XAML 的 x:Static 设置，不是提示文字字符串。
        if (sender is Button { Tag: MeasurementQuality quality })
        {
            // 只修改绑定源，Live.Quality 由绑定自动跟随；保存温度不会被清零。
            SampleQuality = quality;
        }
    }
}
