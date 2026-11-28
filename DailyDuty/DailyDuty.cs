using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using DailyDuty.Addons;
using DailyDuty.Controllers;
using DailyDuty.Utilities;
using Dalamud.IoC;
using Dalamud.Plugin;
using KamiToolKit;

namespace DailyDuty;

public sealed class DailyDuty : IAsyncDalamudPlugin {
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; set; } = null!;

    public async Task LoadAsync(CancellationToken cancellationToken) {
        await KamiToolKitLibrary.InitializeAsync(PluginInterface, "DailyDuty");
        KamiToolKitLibrary.SetResourceManager(Strings.ResourceManager);

        Localization.SetCultureInfo(PluginInterface.UiLanguage);
        PluginInterface.LanguageChanged += Localization.SetCultureInfo;

        System.ConfigWindow = new ConfigWindow {
            InternalName = "DailyDutyConfig",
            Title = Strings.DailyDutyPlugin_Configuration,
            Size = new Vector2(800.0f, 400.0f),
        };

        System.Controller = new SystemController();
        await System.Controller.InitializeAsync();
    }

    public async ValueTask DisposeAsync() {
        await System.Controller.DisposeAsync();
        await System.ConfigWindow.DisposeAsync();
        await KamiToolKitLibrary.DisposeAsync();
    }
}
