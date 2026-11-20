using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TKCode.Arc.Gallery.Catalog;
using TKCode.Arc.Gallery.Demos;
using TKCode.Arc.Gallery.Docs;

namespace TKCode.Arc.Tests.Gallery;

public class LevelIndicatorGalleryTests
{
    [AvaloniaFact]
    public void Demo_Buttons_Update_The_Actual_Repository_Control()
    {
        var demo = new LevelIndicatorReadingsDemo();
        var window = TestHelpers.Show(demo);
        try
        {
            // 演示里只有一个真实组件实例；按钮和滑块只改变它绑定的数据，不复制控件逻辑。
            var meter = demo.GetVisualDescendants().OfType<LevelIndicator>().Single();
            var buttons = demo.GetVisualDescendants().OfType<Button>().ToList();
            var high = buttons.Single(button => Equals(button.Content, "高液位 92 %"));
            ControlAutomationPeer.CreatePeerForElement(high).GetProvider<IInvokeProvider>()!.Invoke();
            Dispatcher.UIThread.RunJobs();
            meter.Value.ShouldBe(92);
            meter.State.ShouldBe(LevelIndicatorState.High);
            var low = buttons.Single(button => Equals(button.Content, "低液位 12 %"));
            ControlAutomationPeer.CreatePeerForElement(low).GetProvider<IInvokeProvider>()!.Invoke();
            Dispatcher.UIThread.RunJobs();
            meter.Value.ShouldBe(12);
            meter.State.ShouldBe(LevelIndicatorState.Low);
            var slider = demo.GetVisualDescendants().OfType<Slider>().Single();
            slider.Value = 100;
            Dispatcher.UIThread.RunJobs();
            meter.Value.ShouldBe(100);
            meter.State.ShouldBe(LevelIndicatorState.High);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Quality_Buttons_Drive_The_Bound_Quality()
    {
        var demo = new LevelIndicatorReadingsDemo();
        var window = TestHelpers.Show(demo);
        try
        {
            var meter = demo.GetVisualDescendants().OfType<LevelIndicator>().Single();
            var buttons = demo.GetVisualDescendants().OfType<Button>().ToList();
            // 质量按钮只改绑定的 SampleQuality，控件据此隐藏液面但保留保存的液位。
            var stale = buttons.Single(button => Equals(button.Content, "数据过期"));
            ControlAutomationPeer.CreatePeerForElement(stale).GetProvider<IInvokeProvider>()!.Invoke();
            Dispatcher.UIThread.RunJobs();
            meter.Quality.ShouldBe(MeasurementQuality.Stale);
            meter.HasReading.ShouldBeFalse();
            var recovered = buttons.Single(button => Equals(button.Content, "恢复数据"));
            ControlAutomationPeer.CreatePeerForElement(recovered).GetProvider<IInvokeProvider>()!.Invoke();
            Dispatcher.UIThread.RunJobs();
            meter.Quality.ShouldBe(MeasurementQuality.Good);
            meter.HasReading.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Component_Has_New_Badge_And_Three_Current_Documentation_Pages()
    {
        var entry = GalleryCatalog.AllEntries.Single(item => item.ClassName == "LevelIndicator");
        entry.IsArc.ShouldBeFalse();
        // 文件目录和侧栏登记必须同时登记，避免代码页仍显示旧目录。
        GalleryCatalog.CustomCharts.Entries.ShouldContain(entry);
        GalleryDemos.Get("LevelIndicator.Readings").ShouldNotBeNull().Files[0].Path
            .ShouldBe("Demos/CustomCharts/LevelIndicator/Readings.axaml");
        entry.IsNewOn(new DateOnly(2026, 11, 17)).ShouldBeTrue();
        foreach (var language in new[] { "en-US", "zh-CN", "zh-TW" })
        {
            var page = GalleryDocs.Get("LevelIndicator", language).ShouldNotBeNull();
            page.IsOutdated.ShouldBeFalse();
            page.Markdown.ShouldContain("### LevelIndicator");
            GalleryDocs.DemoIds("LevelIndicator", language).ShouldContain("LevelIndicator.Readings");
            GalleryDocs.DemoIds("LevelIndicator", language).ShouldContain("LevelIndicator.QualityStates");
        }
    }

    [Fact]
    public void Api_Descriptions_Are_Generated_And_Translated()
    {
        var english = GalleryDocs.Get("LevelIndicator", "en-US").ShouldNotBeNull().Markdown;
        english.ShouldContain("The actual level; an out-of-range reading is kept while the liquid is clamped to the range.");
        english.ShouldContain("The band of the actual reading; changes immediately, independently of animation.");
        var simplified = GalleryDocs.Get("LevelIndicator", "zh-CN").ShouldNotBeNull().Markdown;
        simplified.ShouldContain("| `Value` |");
        simplified.ShouldContain("实际液位");
        var traditional = GalleryDocs.Get("LevelIndicator", "zh-TW").ShouldNotBeNull().Markdown;
        traditional.ShouldContain("| `Value` |");
        traditional.ShouldContain("實際液位");
    }
}
