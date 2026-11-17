using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TKCode.Arc.Gallery.Catalog;
using TKCode.Arc.Gallery.Demos;
using TKCode.Arc.Gallery.Docs;

namespace TKCode.Arc.Tests.Gallery;

public class ThermometerGalleryTests
{
    [AvaloniaFact]
    public void Demo_Buttons_Update_The_Actual_Repository_Control()
    {
        var demo = new ThermometerReadingsDemo();
        var window = TestHelpers.Show(demo);
        try
        {
            var meter = demo.GetVisualDescendants().OfType<Thermometer>().Single();
            var buttons = demo.GetVisualDescendants().OfType<Button>().ToList();
            var high = buttons.Single(button => Equals(button.Content, "高温 86 °C"));
            ControlAutomationPeer.CreatePeerForElement(high).GetProvider<IInvokeProvider>()!.Invoke();
            Dispatcher.UIThread.RunJobs();
            meter.Value.ShouldBe(86);
            meter.State.ShouldBe(ThermometerState.High);
            var low = buttons.Single(button => Equals(button.Content, "低温 −8 °C"));
            ControlAutomationPeer.CreatePeerForElement(low).GetProvider<IInvokeProvider>()!.Invoke();
            Dispatcher.UIThread.RunJobs();
            meter.Value.ShouldBe(-8);
            meter.State.ShouldBe(ThermometerState.Low);
            var slider = demo.GetVisualDescendants().OfType<Slider>().Single();
            slider.Value = 100;
            Dispatcher.UIThread.RunJobs();
            meter.Value.ShouldBe(100);
            meter.State.ShouldBe(ThermometerState.High);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Component_Has_New_Badge_And_Three_Current_Documentation_Pages()
    {
        var entry = GalleryCatalog.AllEntries.Single(item => item.ClassName == "Thermometer");
        entry.IsArc.ShouldBeFalse();
        // 文件目录和侧栏登记必须同时迁移，避免代码页仍显示旧目录。
        GalleryCatalog.CustomCharts.Entries.ShouldContain(entry);
        GalleryDemos.Get("Thermometer.Readings").ShouldNotBeNull().Files[0].Path
            .ShouldBe("Demos/CustomCharts/Thermometer/Readings.axaml");
        entry.IsNewOn(new DateOnly(2026, 11, 16)).ShouldBeTrue();
        foreach (var language in new[] { "en-US", "zh-CN", "zh-TW" })
        {
            var page = GalleryDocs.Get("Thermometer", language).ShouldNotBeNull();
            page.IsOutdated.ShouldBeFalse();
            page.Markdown.ShouldContain("### Thermometer");
            GalleryDocs.DemoIds("Thermometer", language).ShouldContain("Thermometer.Readings");
        }
    }

    [Fact]
    public void Api_Descriptions_Are_Generated_And_Translated()
    {
        var english = GalleryDocs.Get("Thermometer", "en-US").ShouldNotBeNull().Markdown;
        english.ShouldContain("The actual temperature; an out-of-range reading is kept while the liquid is clamped.");
        english.ShouldContain("The band of the actual reading; changes immediately, independently of animation.");
        var simplified = GalleryDocs.Get("Thermometer", "zh-CN").ShouldNotBeNull().Markdown;
        simplified.ShouldContain("| `Value` |");
        simplified.ShouldContain("实际温度");
        var traditional = GalleryDocs.Get("Thermometer", "zh-TW").ShouldNotBeNull().Markdown;
        traditional.ShouldContain("| `Value` |");
        traditional.ShouldContain("實際溫度");
    }
}
