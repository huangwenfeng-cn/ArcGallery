using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TKCode.Arc.Gallery.Catalog;
using TKCode.Arc.Gallery.Demos;
using TKCode.Arc.Gallery.Docs;

namespace TKCode.Arc.Tests.Gallery;

public class WindDirectionIndicatorGalleryTests
{
    [AvaloniaFact]
    public void Demo_Buttons_And_Slider_Update_One_Repository_Compass()
    {
        var demo = new WindDirectionIndicatorReadingsDemo();
        var window = TestHelpers.Show(demo);
        try
        {
            var meter = demo.GetVisualDescendants().OfType<WindDirectionIndicator>().Single();
            void Invoke(string text)
            {
                // 通过按钮的无障碍调用接口触发真实 Click，覆盖 XAML 与后台事件连接。
                var button = demo.GetVisualDescendants().OfType<Button>().Single(item => Equals(item.Content, text));
                ControlAutomationPeer.CreatePeerForElement(button).GetProvider<IInvokeProvider>()!.Invoke();
                Dispatcher.UIThread.RunJobs();
            }
            Invoke("北向 359°");
            meter.Direction.ShouldBe(359);
            Invoke("北向 1°");
            meter.Direction.ShouldBe(1);
            Invoke("静风");
            meter.IsCalm.ShouldBeTrue();
            Invoke("无数据");
            meter.HasData.ShouldBeFalse();
            var slider = demo.GetVisualDescendants().OfType<Slider>().Single();
            slider.Value = 270;
            meter.Direction.ShouldBe(270);
            meter.HasDirection.ShouldBeTrue();
            Invoke("方向读数");
            meter.ShowReading.ShouldBeTrue();
            Invoke("方向读数");
            meter.ShowReading.ShouldBeFalse();
            Invoke("静风");
            Invoke("北向 1°");
            meter.HasDirection.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Component_Has_A_New_Badge_And_Current_Translated_Docs_And_Api()
    {
        var entry = GalleryCatalog.AllEntries.Single(item => item.ClassName == "WindDirectionIndicator");
        entry.IsArc.ShouldBeFalse();
        // 验证分类身份与生成的演示源码路径，防止文件迁移后侧栏或代码页遗漏更新。
        GalleryCatalog.CustomCharts.Entries.ShouldContain(entry);
        GalleryDemos.Get("WindDirectionIndicator.Readings").ShouldNotBeNull().Files[0].Path
            .ShouldBe("Demos/CustomCharts/WindDirectionIndicator/Readings.axaml");
        entry.IsNewOn(new DateOnly(2026, 11, 17)).ShouldBeTrue();
        foreach (var language in new[] { "en-US", "zh-CN", "zh-TW" })
        {
            // 检查生成后的页面，既验证三语资源，也验证公共 XML 文档形成的 API 表。
            var page = GalleryDocs.Get("WindDirectionIndicator", language).ShouldNotBeNull();
            page.IsOutdated.ShouldBeFalse();
            page.Markdown.ShouldContain("### WindDirectionIndicator");
            page.Markdown.ShouldContain("| `HasData` |");
            page.Markdown.ShouldContain("| `ShowReading` |");
            page.Markdown.ShouldNotContain("| `Speed` |");
            GalleryDocs.DemoIds("WindDirectionIndicator", language).ShouldContain("WindDirectionIndicator.Readings");
        }
        GalleryDocs.Get("WindDirectionIndicator", "zh-CN")!.Markdown.ShouldContain("控件不会推断静风阈值");
        GalleryDocs.Get("WindDirectionIndicator", "zh-TW")!.Markdown.ShouldContain("控制項不會推斷靜風閾值");
    }
}
