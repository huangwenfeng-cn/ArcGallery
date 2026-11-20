using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TKCode.Arc.Gallery.Demos;
using TKCode.Arc.Gallery.Docs;

namespace TKCode.Arc.Tests.Gallery;

public class MeasurementQualityGalleryTests
{
    // 这些测试验证 XAML 绑定和按钮事件真实连通，不只是验证控件单独设置属性。
    // IInvokeProvider 模拟触发按钮的 Click，RunJobs 处理 UI 线程排队的更新，
    // 然后同时检查演示数据源和实际控件。测试窗口在 finally 中关闭。
    [AvaloniaFact]
    public void Thermometer_Demo_Binds_Quality_And_Same_Value_Button_Recovers()
    {
        var demo = new ThermometerReadingsDemo();
        var window = TestHelpers.Show(demo);
        try
        {
            var meter = demo.GetVisualDescendants().OfType<Thermometer>().Single();
            foreach (var quality in Enum.GetValues<MeasurementQuality>())
            {
                var button = demo.GetVisualDescendants().OfType<Button>().Single(b => Equals(b.Tag, quality));
                ControlAutomationPeer.CreatePeerForElement(button).GetProvider<IInvokeProvider>()!.Invoke();
                Dispatcher.UIThread.RunJobs();
                demo.SampleQuality.ShouldBe(quality);
                meter.Quality.ShouldBe(quality);
            }
            demo.SampleQuality = MeasurementQuality.Error;
            Dispatcher.UIThread.RunJobs();
            var normal = demo.GetVisualDescendants().OfType<Button>().Single(b => Equals(b.Content, "正常 36.5 °C"));
            ControlAutomationPeer.CreatePeerForElement(normal).GetProvider<IInvokeProvider>()!.Invoke();
            Dispatcher.UIThread.RunJobs();
            meter.Value.ShouldBe(36.5);
            meter.Quality.ShouldBe(MeasurementQuality.Good);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Compass_Demo_Binds_Quality_And_Recovery_Clears_Missing_And_Calm()
    {
        var demo = new WindDirectionIndicatorReadingsDemo();
        var window = TestHelpers.Show(demo);
        try
        {
            var meter = demo.GetVisualDescendants().OfType<WindDirectionIndicator>().Single();
            foreach (var quality in new[] { MeasurementQuality.Stale, MeasurementQuality.Error, MeasurementQuality.Good })
            {
                meter.HasData = false;
                meter.IsCalm = true;
                var button = demo.GetVisualDescendants().OfType<Button>().Single(b => Equals(b.Tag, quality));
                ControlAutomationPeer.CreatePeerForElement(button).GetProvider<IInvokeProvider>()!.Invoke();
                Dispatcher.UIThread.RunJobs();
                demo.SampleQuality.ShouldBe(quality);
                meter.Quality.ShouldBe(quality);
                meter.HasData.ShouldBeTrue();
                meter.IsCalm.ShouldBeFalse();
            }
            demo.SampleQuality = MeasurementQuality.Error;
            Dispatcher.UIThread.RunJobs();
            meter.Quality.ShouldBe(MeasurementQuality.Error);
            demo.GetVisualDescendants().OfType<Slider>().Single().Value = 90;
            Dispatcher.UIThread.RunJobs();
            meter.Quality.ShouldBe(MeasurementQuality.Good);
            meter.Direction.ShouldBe(90);
        }
        finally
        {
            window.Close();
        }
    }

    [Theory]
    [InlineData("Thermometer")]
    [InlineData("WindDirectionIndicator")]
    public void Quality_Api_And_Comparison_Are_In_All_Current_Pages(string component)
    {
        foreach (var language in new[] { "en-US", "zh-CN", "zh-TW" })
        {
            var page = GalleryDocs.Get(component, language).ShouldNotBeNull();
            page.IsOutdated.ShouldBeFalse();
            page.Markdown.ShouldContain("| `Quality` |");
            page.Markdown.ShouldContain("| `StaleLabel` |");
            page.Markdown.ShouldContain("| `ErrorLabel` |");
            GalleryDocs.DemoIds(component, language).ShouldContain($"{component}.QualityStates");
        }
    }
}
