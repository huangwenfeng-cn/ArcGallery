using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TKCode.Arc.Gallery.Catalog;
using TKCode.Arc.Gallery.Demos;
using TKCode.Arc.Gallery.Docs;

namespace TKCode.Arc.Tests.Gallery;

public class StatusIndicatorGalleryTests
{
    [AvaloniaFact]
    public void Demo_Changes_Feedback_And_Binds_Existing_Switches()
    {
        var demo = new StatusIndicatorStatesDemo();
        var window = TestHelpers.Show(demo);
        try
        {
            var target = demo.GetVisualDescendants().OfType<StatusIndicator>().Single(c => c.Name == "Target");
            foreach (var state in Enum.GetValues<StatusIndicatorState>())
            {
                var button = demo.GetVisualDescendants().OfType<Button>().Single(b => Equals(b.Tag, state));
                ControlAutomationPeer.CreatePeerForElement(button).GetProvider<IInvokeProvider>()!.Invoke();
                Dispatcher.UIThread.RunJobs();
                demo.FeedbackState.ShouldBe(state);
                target.State.ShouldBe(state);
            }
            // 模拟外部采样改变数据源，验证 State 确实由绑定更新。
            demo.FeedbackState = StatusIndicatorState.Running;
            Dispatcher.UIThread.RunJobs();
            target.State.ShouldBe(StatusIndicatorState.Running);
            demo.GetVisualDescendants().OfType<Switch>().Single(s => s.Name == "Blink").IsChecked = true;
            demo.GetVisualDescendants().OfType<Switch>().Single(s => s.Name == "Labels").IsChecked = false;
            Dispatcher.UIThread.RunJobs();
            target.IsBlinking.ShouldBeTrue();
            target.ShowLabel.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void New_Component_Has_Three_Current_Pages_Api_And_CustomCharts_Demo()
    {
        var entry = GalleryCatalog.AllEntries.Single(e => e.ClassName == "StatusIndicator");
        GalleryCatalog.CustomCharts.Entries.ShouldContain(entry);
        entry.IsArc.ShouldBeFalse();
        entry.IsNewOn(new DateOnly(2026, 11, 17)).ShouldBeTrue();
        GalleryDocs.PackageOf("StatusIndicator").ShouldBe("TKCode.Arc");
        GalleryDemos.Get("StatusIndicator.States").ShouldNotBeNull().Files[0].Path
            .ShouldBe("Demos/CustomCharts/StatusIndicator/States.axaml");
        foreach (var language in new[] { "en-US", "zh-CN", "zh-TW" })
        {
            var page = GalleryDocs.Get("StatusIndicator", language).ShouldNotBeNull();
            page.IsOutdated.ShouldBeFalse();
            page.Markdown.ShouldContain("| `State` |");
            page.Markdown.ShouldContain("| `DisplayText` |");
            GalleryDocs.DemoIds("StatusIndicator", language).ShouldContain("StatusIndicator.States");
        }
    }
}
