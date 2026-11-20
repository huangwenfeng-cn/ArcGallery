using TKCode.Arc.Gallery.Localization;
using TKCode.Arc.Gallery.Shell;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using TKCode.Arc.Gallery.Catalog;

namespace TKCode.Arc.Gallery.Desktop;

public partial class App : Application
{
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            GalleryHost.Kind = GalleryHostKind.Desktop;
            GalleryUpdates.Store = new FileUpdateStore();
            var window = new GalleryWindow();
            window.Opened += (_, _) =>
            {
                MacDockIcon.Apply();
                // 本地预览可用 --component LinearGauge 直接定位页面。
                // 只查现有目录，不加载外部类型；未提供参数时沿用默认首页。
                var args = desktop.Args ?? [];
                var componentIndex = Array.IndexOf(args, "--component");
                if (componentIndex >= 0 && componentIndex + 1 < args.Length)
                {
                    var entry = GalleryCatalog.AllEntries.FirstOrDefault(item =>
                        string.Equals(item.ClassName, args[componentIndex + 1], StringComparison.OrdinalIgnoreCase));
                    if (entry is not null)
                    {
                        window.Select(entry);
                    }
                }
                _ = NotifyUpdateAsync(window);
            };
            desktop.MainWindow = window;
        }
        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>Unless turned off in About, checks at startup and shows a toast with a download button when GitHub has a newer release.</summary>
    private static async Task NotifyUpdateAsync(GalleryWindow window)
    {
        if (!GalleryUpdates.AutoCheck)
        {
            return;
        }
        await GalleryUpdates.CheckAsync();
        if (GalleryUpdates.Available is not { } release)
        {
            return;
        }
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var download = new ToastAction(GalleryText.Get(GalleryShellLangResourceKind.UpdateDownload),
                                           id => { _ = window.Launcher.LaunchUriAsync(release.DownloadUrl); });
            Toast.Show(window, GalleryText.Format(GalleryShellLangResourceKind.UpdateAvailable, release.Version),
                       action: download, duration: TimeSpan.FromSeconds(30));
        });
    }
}
