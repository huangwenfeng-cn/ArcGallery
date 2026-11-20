using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TKCode.Arc.Gallery.Demos;

public partial class WindDirectionIndicatorReadingsDemo : UserControl
{
    // 用带通知的属性模拟 ViewModel。XAML 的 #Feedback 指向本 UserControl，
    // #Feedback.SampleQuality 改变后，会通过绑定更新 Live.Quality。
    // 演示按钮只模拟反馈，不执行实际采集、超时判断或设备控制。
    public static readonly StyledProperty<MeasurementQuality> SampleQualityProperty =
        AvaloniaProperty.Register<WindDirectionIndicatorReadingsDemo, MeasurementQuality>(nameof(SampleQuality));

    public MeasurementQuality SampleQuality
    {
        get => GetValue(SampleQualityProperty);
        set => SetValue(SampleQualityProperty, value);
    }

    public WindDirectionIndicatorReadingsDemo()
    {
        // 编译 XAML 生成 InitializeComponent 和 Live/Bearing 字段；先加载界面再访问命名控件。
        InitializeComponent();
        // 订阅本示例拥有的滑块，二者生命周期一致，不涉及外部全局事件的长寿命引用。
        Bearing.PropertyChanged += (_, change) =>
        {
            if (change.Property == Slider.ValueProperty)
            {
                SetReading(Bearing.Value);
            }
        };
    }

    private void SetReading(double direction)
    {
        // 模拟一次有效采样：实际方向、静风状态和数据质量分别由业务层明确提供。
        // 移动滑块也通过这里恢复正常读数，不能只改角度却保留“无数据”状态。
        // 先给出本次真实方向，再解除无效质量/静风/缺测标记，恢复时才能显示新方向。
        Live.Direction = direction;
        Live.IsCalm = false;
        SampleQuality = MeasurementQuality.Good;
        Live.HasData = true;
    }

    private void OnBeforeNorthClick(object? sender, RoutedEventArgs e)
    {
        Bearing.Value = 359;
        // 即使滑块已经是 359，属性也不会再次通知；显式调用仍可从静风/缺测恢复。
        SetReading(359);
    }

    private void OnAfterNorthClick(object? sender, RoutedEventArgs e)
    {
        Bearing.Value = 1;
        // 与 359° 按钮配合展示跨北时的最短旋转；同值点击也要恢复有效采样。
        SetReading(1);
    }

    private void OnCalmClick(object? sender, RoutedEventArgs e)
    {
        // 静风是业务判定，不根据某个风向角度推断；这里用按钮模拟业务输入。
        Live.IsCalm = true;
        SampleQuality = MeasurementQuality.Good;
        Live.HasData = true;
    }

    private void OnNoDataClick(object? sender, RoutedEventArgs e) => Live.HasData = false;

    private void OnQualityClick(object? sender, RoutedEventArgs e)
    {
        // Tag 保存 MeasurementQuality 枚举，模式匹配把它取出到 quality。
        // 不通过按钮文字猜测状态，所以修改按钮文字不会改变对应的质量含义。
        if (sender is Button { Tag: MeasurementQuality quality })
        {
            SampleQuality = quality;
            // 显式清除旧标记，否则 HasData=false 会继续优先显示无数据，
            // 或在恢复 Good 后继续显示静风，让质量按钮看起来没有生效。
            Live.IsCalm = false;
            Live.HasData = true;
        }
    }

    // 只切换可见读数及其布局空间；自动化接口始终能够读取实际状态。
    private void OnReadingClick(object? sender, RoutedEventArgs e) => Live.ShowReading = !Live.ShowReading;
}
