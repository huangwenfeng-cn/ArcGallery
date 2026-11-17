using Avalonia.Headless.XUnit;
using TKCode.Arc.Gallery;
using TKCode.Arc.Gallery.Docs;

namespace TKCode.Arc.Tests.Gallery;

/// <summary>验证拆包后的文档包归属和 Gallery 的高亮注册。</summary>
public class GalleryMarkdownPackageTests
{
    [AvaloniaFact]
    public void Docs_Report_The_Package_That_Ships_Each_Control()
    {
        foreach (var name in new[] { "Markdown", "ChatThread" })
        {
            GalleryDocs.PackageOf(name).ShouldBe("TKCode.Arc.Markdown", name);
        }
        typeof(ChatComposer).Assembly.GetName().Name.ShouldBe("TKCode.Arc.Markdown");
        GalleryDocs.PackageOf("CodeBlock").ShouldBe("TKCode.Arc");
        GalleryDocs.PackageOf("Thermometer").ShouldBe("TKCode.Arc");
        GalleryDocs.PackageOf("WindDirectionIndicator").ShouldBe("TKCode.Arc");
    }

    [AvaloniaFact]
    public void Gallery_Enables_Markdown_And_Produces_Colored_Code()
    {
        var builder = new ArcBuilder().UseArcGallery();
        builder.IsEnabled("TKCode.Arc.Markdown").ShouldBeTrue();
        builder.IsEnabled("TKCode.Arc.Highlighting").ShouldBeTrue();
        // 检查运行时桥接实际产生语法角色，防止只有引用而没有启用高亮。
        using var session = CodeBlockHighlighter.CreateSession("csharp");
        session.ShouldNotBeNull();
        var spans = session.Update("public class Sample { }").SelectMany(line => line).ToList();
        spans.ShouldContain(span => span.Kind == CodeBlockSyntaxKind.Keyword);
        spans.ShouldContain(span => span.Kind == CodeBlockSyntaxKind.Type);
    }
}
