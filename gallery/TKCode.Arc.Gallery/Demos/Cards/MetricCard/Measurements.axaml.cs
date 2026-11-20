using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace TKCode.Arc.Gallery.Demos;

// 这是业务组合示例，不是新增公共控件。可以依次阅读：
// 1. 演示按钮怎样模拟采样；2. 数据源怎样保存数值和质量；3. 快照怎样决定使用哪种 XAML 模板。
// 文案是此中文演示的数据，实际应用应从自己的语言资源提供标签和质量文字。
public partial class MetricCardMeasurementsDemo : UserControl
{
    public MeasurementReadoutSource WindSpeed { get; } = new("风速", " m/s", 1);
    public MeasurementReadoutSource Humidity { get; } = new("相对湿度", "%", 1);
    public MeasurementReadoutSource Pressure { get; } = new("气压", " hPa", 1);

    public MetricCardMeasurementsDemo()
    {
        // 数据源先由属性初始化创建，XAML 加载后才把当前视图作为编译绑定的来源。
        // 初始质量为 NoData：尚未采样时显示“无数据”，不会误显示默认 double 的零。
        InitializeComponent();
        DataContext = this;
    }

    private void OnSampleClick(object? sender, RoutedEventArgs e)
    {
        // 使用固定样本便于复现。重复点击也可从 Error 恢复到相同的有效读数。
        // 实际网关回调应 Dispatcher.UIThread.Post 到 UI 线程后，再调用 SetSample。
        WindSpeed.SetSample(7.2, MeasurementQuality.Good);
        Humidity.SetSample(56.4, MeasurementQuality.Good);
        Pressure.SetSample(1013.2, MeasurementQuality.Good);
    }

    private void OnZeroWindClick(object? sender, RoutedEventArgs e)
    {
        // 零是合法的测量结果；是否静风、是否报警由业务层判定，不能将零等同于无数据。
        WindSpeed.SetSample(0, MeasurementQuality.Good);
    }

    private void OnQualityClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: MeasurementQuality quality })
        {
            // 只改变质量，不修改最后的有限数值。枚举放在 Tag 中，避免按按钮文案判断状态。
            WindSpeed.SetQuality(quality);
            Humidity.SetQuality(quality);
            Pressure.SetQuality(quality);
        }
    }
}

// 数据源属于演示视图，只有普通属性通知，没有外部长寿命订阅或计时器。
// View 卸载后 Avalonia 解除绑定；数据源也随 View 回收，不需要另加 Dispose 或挂载标志。
public sealed class MeasurementReadoutSource : INotifyPropertyChanged
{
    public string Label { get; }
    public string Unit { get; }
    public int Decimals { get; }

    // null 表示从未收到有限数值。保存值供业务层使用，非 Good 时不会交给数值模板。
    public double? SavedValue { get; private set; }
    public MeasurementQuality Quality { get; private set; } = MeasurementQuality.NoData;
    public object Display { get; private set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    public MeasurementReadoutSource(string label, string unit, int decimals)
    {
        Label = label;
        Unit = unit;
        Decimals = decimals;
        Display = new MeasurementUnavailableSnapshot(label, MeasurementQuality.NoData);
    }

    public void SetSample(double value, MeasurementQuality quality)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (double.IsFinite(value))
        {
            SavedValue = value;
        }
        else
        {
            // 即使采集方误标 Good，NaN/Infinity 也不能作为当前测量显示。
            // 保留最后有限值，降级为 Error；不凭空制造一个零来顶替异常样本。
            quality = MeasurementQuality.Error;
        }
        Publish(quality);
    }

    public void SetQuality(MeasurementQuality quality)
    {
        Dispatcher.UIThread.VerifyAccess();
        Publish(quality);
    }

    private void Publish(MeasurementQuality quality)
    {
        // 先完成所有内部字段，再发一次“所有属性变化”的通知（null 是该接口约定）。
        // 这样绑定观察到的是同一份完整状态，不会先出现 Good、再看见上一次的数字。
        // 恢复时应调用 SetSample(新值, Good)，而不是单独将保存旧值的质量改回 Good。
        Quality = Enum.IsDefined(quality) ? quality : MeasurementQuality.Error;
        if (Quality == MeasurementQuality.Good && SavedValue is null)
        {
            Quality = MeasurementQuality.NoData;
        }
        Display = Quality == MeasurementQuality.Good
            ? new MeasurementNumberSnapshot(Label, SavedValue!.Value, Unit, Decimals)
            : new MeasurementUnavailableSnapshot(Label, Quality);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}

// 不可变快照把两种展示数据分开：有效快照包含数字，无效快照根本没有数字属性。
// ContentControl 按运行时类型挑选 DataTemplate，整个旧数值控件会被替换。
public sealed record MeasurementNumberSnapshot(string Label, double Value, string Unit, int Decimals)
{
    public string Context => "有效数据";
}

public sealed record MeasurementUnavailableSnapshot(string Label, MeasurementQuality Quality)
{
    public bool IsStale => Quality == MeasurementQuality.Stale;
    public bool IsError => Quality == MeasurementQuality.Error;
    public string Context => Quality switch
    {
        MeasurementQuality.Stale => "数据过期",
        MeasurementQuality.Error => "采集异常",
        _ => "无数据"
    };
}
