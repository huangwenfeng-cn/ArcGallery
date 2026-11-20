using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TKCode.Arc.Gallery.Demos;
using TKCode.Arc.Gallery.Docs;
using TKCode.Arc.Primitives;

namespace TKCode.Arc.Tests.Gallery;

public class MeasurementReadoutGalleryTests
{
    // 验证的是实际 XAML 绑定、按钮事件和模板替换，而非只检查 ViewModel 字段。
    // Show 会建立真实的 headless 控件树；finally 关闭窗口，避免影响后续用例。
    [AvaloniaFact]
    public void Quality_Buttons_Remove_Old_Numbers_And_Same_Sample_Recovers()
    {
        var demo = new MetricCardMeasurementsDemo();
        var window = TestHelpers.Show(demo);
        try
        {
            Cards(demo).ShouldBeEmpty();
            demo.WindSpeed.SavedValue.ShouldBeNull();
            Click(demo, "采样 / 恢复");
            Cards(demo).Select(c => c.Value).ShouldBe([7.2, 56.4, 1013.2]);
            Cards(demo).Select(c => c.Suffix).ShouldBe([" m/s", "%", " hPa"]);
            Cards(demo).ShouldAllBe(c => c.Decimals == 1 && c.Change == null && c.Sparkline == null);

            foreach (var quality in new[] { MeasurementQuality.NoData, MeasurementQuality.Stale, MeasurementQuality.Error })
            {
                var button = demo.GetVisualDescendants().OfType<Button>().Single(b => Equals(b.Tag, quality));
                ControlAutomationPeer.CreatePeerForElement(button).GetProvider<IInvokeProvider>()!.Invoke();
                Dispatcher.UIThread.RunJobs();
                demo.WindSpeed.Quality.ShouldBe(quality);
                demo.WindSpeed.SavedValue.ShouldBe(7.2);
                Cards(demo).ShouldBeEmpty();
                demo.GetVisualDescendants().OfType<AnimatedCounter>().ShouldBeEmpty();
                // 旧数字控件已离开树，当前根节点的自动化子树也不能再朗读该数字。
                var names = Names(ControlAutomationPeer.CreatePeerForElement(demo)).ToArray();
                names.ShouldNotContain(n => n.Contains("7.2", StringComparison.Ordinal));
                names.ShouldContain(((MeasurementUnavailableSnapshot)demo.WindSpeed.Display).Context);
                Click(demo, "采样 / 恢复");
                Cards(demo).Select(c => c.Value).ShouldBe([7.2, 56.4, 1013.2]);
                demo.WindSpeed.Quality.ShouldBe(MeasurementQuality.Good);
            }
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Zero_Is_Valid_And_Invalid_Samples_Keep_The_Last_Finite_Value()
    {
        var demo = new MetricCardMeasurementsDemo();
        var window = TestHelpers.Show(demo);
        try
        {
            Click(demo, "零风速");
            Cards(demo).Single().Value.ShouldBe(0);
            demo.WindSpeed.Quality.ShouldBe(MeasurementQuality.Good);
            foreach (var value in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity })
            {
                demo.WindSpeed.SetSample(value, MeasurementQuality.Good);
                Dispatcher.UIThread.RunJobs();
                demo.WindSpeed.Quality.ShouldBe(MeasurementQuality.Error);
                demo.WindSpeed.SavedValue.ShouldBe(0);
                Cards(demo).ShouldBeEmpty();
                demo.WindSpeed.SetSample(0, MeasurementQuality.Good);
                Dispatcher.UIThread.RunJobs();
                Cards(demo).Single().Value.ShouldBe(0);
            }
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Notifications_Expose_Complete_Snapshots_And_Remount_Reads_Latest_Data()
    {
        var demo = new MetricCardMeasurementsDemo();
        var source = demo.WindSpeed;
        source.SetQuality(MeasurementQuality.Good);
        source.Quality.ShouldBe(MeasurementQuality.NoData);
        var notifications = 0;
        source.PropertyChanged += (_, _) =>
        {
            // 通知接收者不能观察到新质量搭配旧数值。这里只订阅局部测试对象。
            notifications++;
            if (source.Quality == MeasurementQuality.Good)
            {
                source.Display.ShouldBeOfType<MeasurementNumberSnapshot>().Value.ShouldBe(source.SavedValue!.Value);
            }
            else
            {
                source.Display.ShouldBeOfType<MeasurementUnavailableSnapshot>().Quality.ShouldBe(source.Quality);
            }
        };
        source.SetSample(7.2, MeasurementQuality.Good);
        source.SetSample(8.1, MeasurementQuality.Stale);
        source.SavedValue.ShouldBe(8.1);
        source.SetQuality((MeasurementQuality)999);
        source.Quality.ShouldBe(MeasurementQuality.Error);
        source.SetSample(9.3, MeasurementQuality.Good);
        notifications.ShouldBe(4);

        var window = TestHelpers.Show(demo);
        try
        {
            Cards(demo).Single().Value.ShouldBe(9.3);
            window.Content = null;
            Dispatcher.UIThread.RunJobs();
            source.SetSample(10.4, MeasurementQuality.Good);
            window.Content = demo;
            Dispatcher.UIThread.RunJobs();
            Cards(demo).Single().Value.ShouldBe(10.4);
            // Off 应立即呈现目标，不留下从零入场或恢复时的中间数位。
            var counter = demo.GetVisualDescendants().OfType<AnimatedCounter>().Single();
            ControlAutomationPeer.CreatePeerForElement(counter).GetName().ShouldContain("10.4");
            counter.GetVisualDescendants().OfType<RollingDigits>().Single().DisplayText.ShouldBe("10.4 m/s");
        }
        finally
        {
            window.Close();
        }
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("zh-CN")]
    [InlineData("zh-TW")]
    public void Measurement_Demo_Is_In_Current_Translations(string language)
    {
        GalleryDocs.Get("MetricCard", language).ShouldNotBeNull().IsOutdated.ShouldBeFalse();
        GalleryDocs.DemoIds("MetricCard", language).ShouldContain("MetricCard.Measurements");
    }

    private static MetricCard[] Cards(UserControl demo) => demo.GetVisualDescendants().OfType<MetricCard>().ToArray();

    private static void Click(UserControl demo, string text)
    {
        var button = demo.GetVisualDescendants().OfType<Button>().Single(b => Equals(b.Content, text));
        ControlAutomationPeer.CreatePeerForElement(button).GetProvider<IInvokeProvider>()!.Invoke();
        Dispatcher.UIThread.RunJobs();
    }

    private static IEnumerable<string> Names(AutomationPeer peer)
    {
        yield return peer.GetName() ?? string.Empty;
        foreach (var child in peer.GetChildren() ?? [])
        {
            foreach (var name in Names(child))
            {
                yield return name;
            }
        }
    }
}
