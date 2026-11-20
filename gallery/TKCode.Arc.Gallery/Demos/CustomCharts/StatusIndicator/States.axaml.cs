using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace TKCode.Arc.Gallery.Demos;

public partial class StatusIndicatorStatesDemo : UserControl
{
    // 可观察表示“改变时会通知绑定”。XAML 的 #Feedback 指向本 UserControl，
    // 灯的 State 绑定到它的 FeedbackState；按钮修改数据源，灯随绑定更新。
    // 实际接入时用业务 ViewModel 的通知属性承接网关/数据库反馈。
    public static readonly StyledProperty<StatusIndicatorState> FeedbackStateProperty =
        AvaloniaProperty.Register<StatusIndicatorStatesDemo, StatusIndicatorState>(
            nameof(FeedbackState), StatusIndicatorState.Running);

    public StatusIndicatorState FeedbackState
    {
        get => GetValue(FeedbackStateProperty);
        set => SetValue(FeedbackStateProperty, value);
    }

    // 加载对应 XAML，建立控件树、事件连接和绑定；不是在构造时连接设备。
    public StatusIndicatorStatesDemo() => AvaloniaXamlLoader.Load(this);

    private void OnStateClick(object? sender, RoutedEventArgs e)
    {
        // sender 是触发 Click 的控件。确认它是 Button，并从 Tag 取出状态枚举；
        // enum 值来自 XAML 的 x:Static，不依赖按钮显示的文字。
        if (sender is Button { Tag: StatusIndicatorState state })
        {
            FeedbackState = state;
        }
    }
}
