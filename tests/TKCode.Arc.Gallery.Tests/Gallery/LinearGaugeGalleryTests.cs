using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TKCode.Arc.Gallery.Catalog;
using TKCode.Arc.Gallery.Demos;
using TKCode.Arc.Gallery.Docs;

namespace TKCode.Arc.Tests.Gallery;

public class LinearGaugeGalleryTests
{
    [AvaloniaFact]
    public void Demo_Updates_Only_Force_And_Repeating_A_Reading_Restores_Bound_Quality()
    {
        var demo = new LinearGaugeReadingsDemo();
        var window = TestHelpers.Show(demo);
        try
        {
            var meters = demo.GetVisualDescendants().OfType<LinearGauge>().ToList();
            meters.Count.ShouldBe(3);
            var force = meters.Single(meter => meter.Unit == "N");
            var buttons = demo.GetVisualDescendants().OfType<Button>().ToList();
            void Press(string content)
            {
                ControlAutomationPeer.CreatePeerForElement(buttons.Single(button => Equals(button.Content, content)))
                    .GetProvider<IInvokeProvider>()!.Invoke();
                Dispatcher.UIThread.RunJobs();
            }
            Press("满量程 1000 N");
            force.Value.ShouldBe(1000);
            force.State.ShouldBe(LinearGaugeState.High);
            Press("数据过期");
            force.Quality.ShouldBe(MeasurementQuality.Stale);
            force.ReadingText.ShouldBe("—");
            Press("满量程 1000 N");
            force.Quality.ShouldBe(MeasurementQuality.Good);
            force.DrawnFraction.ShouldBe(1);
            meters.Single(meter => meter.Unit == "MPa").Value.ShouldBe(.65);
            meters.Single(meter => meter.Unit == "rpm").Value.ShouldBe(1800);
            demo.GetVisualDescendants().OfType<Slider>().Single().Value = 120;
            Dispatcher.UIThread.RunJobs();
            force.Value.ShouldBe(120);
            force.State.ShouldBe(LinearGaugeState.Low);
        }
        finally { window.Close(); }
    }
    [Fact]
    public void Catalog_Docs_Demo_And_Translated_Api_Are_Registered()
    {
        var entry = GalleryCatalog.CustomCharts.Entries.Single(item => item.ClassName == "LinearGauge");
        entry.IsNewOn(new DateOnly(2026, 11, 17)).ShouldBeTrue();
        entry.ArcUrl.ShouldBeNull();
        GalleryDemos.Get("LinearGauge.Readings").ShouldNotBeNull();
        foreach (var language in new[] { "en-US", "zh-CN", "zh-TW" })
        {
            var page = GalleryDocs.Get("LinearGauge", language).ShouldNotBeNull();
            page.IsOutdated.ShouldBeFalse();
            page.Markdown.ShouldContain("### LinearGauge");
            GalleryDocs.DemoIds("LinearGauge", language).ShouldContain("LinearGauge.Readings");
        }
        GalleryDocs.Get("LinearGauge", "zh-CN")!.Markdown.ShouldContain("实际读数");
        GalleryDocs.Get("LinearGauge", "zh-TW")!.Markdown.ShouldContain("實際讀值");
    }
}
