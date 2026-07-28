using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Utilities;
using DailyDuty.Windows;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KamiToolKit;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IAsyncDalamudPlugin {
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; set; } = null!;

    public Task LoadAsync(CancellationToken cancellationToken) {
        KamiToolKitLibrary.Initialize(PluginInterface, "DailyDuty");
        KamiToolKitLibrary.SetResourceManager(Strings.ResourceManager);

        Localization.SetCultureInfo(PluginInterface.UiLanguage);
        PluginInterface.LanguageChanged += Localization.SetCultureInfo;

        System.ConfigurationWindow = new ModuleBrowserWindow {
            InternalName = "DailyDutyConfig",
            Title = Strings.DailyDutyPlugin_Configuration,
            Size = new Vector2(700.0f, 600.0f),
        };

        ICommandManager.Get().AddHandler("/dd", new CommandInfo(OnCommandReceived) {
            HelpMessage = Strings.DutyFinderEnhancements_OpenDailyDuty,
            ShowInHelp = true,
        });

        ICommandManager.Get().AddHandler("/dailyduty", new CommandInfo(OnCommandReceived) {
            HelpMessage = Strings.DutyFinderEnhancements_OpenDailyDuty,
            ShowInHelp = true,
        });

        System.PayloadController = new PayloadController();
        System.ModuleManager = new ModuleManager();

        if (IClientState.Get().IsLoggedIn) {
            OnLogin();
        }

        IClientState.Get().Login += OnLogin;
        IClientState.Get().Logout += OnLogout;

        PluginInterface.UiBuilder.OpenConfigUi += System.ConfigurationWindow.Toggle;
        PluginInterface.UiBuilder.OpenMainUi += System.ConfigurationWindow.Toggle;

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync() {
        PluginInterface.UiBuilder.OpenConfigUi -= System.ConfigurationWindow.Toggle;
        PluginInterface.UiBuilder.OpenMainUi -= System.ConfigurationWindow.Toggle;

        IClientState.Get().Login -= OnLogin;
        IClientState.Get().Logout -= OnLogout;

        ICommandManager.Get().RemoveHandler("/dd");
        ICommandManager.Get().RemoveHandler("/dailyduty");

        System.PayloadController.Dispose();

        await System.ConfigurationWindow.DisposeAsync();
        await System.ModuleManager.DisposeAsync();
        await IFramework.Get().RunOnFrameworkThread(KamiToolKitLibrary.Dispose);
    }

    private static void OnCommandReceived(string command, string arguments) {
        if (command is not ("/dailyduty" or "/dd")) return;

        switch (arguments.Split(" ")) {
            case [ "" ] or [] or null:
                System.ConfigurationWindow.Toggle();
                break;

            case [ "logevents" ] when System.SystemConfig is not null:
                System.SystemConfig.EnableSceneEventLogging = !System.SystemConfig.EnableSceneEventLogging;
                var enabled = Strings.EventLogging_Enabled;
                var disabled = Strings.CompletionStatus_Disabled;
                var message = Strings.EventLogging_Status;
                IChatGui.Get().Print($"{message} {(System.SystemConfig.EnableSceneEventLogging ? enabled : disabled)}", "DailyDuty");
                IPluginLog.Get().Info($"Event is now {(System.SystemConfig.EnableSceneEventLogging ? "Enabled" : "Disabled")}");
                Task.Run(System.SystemConfig.Save);
                break;
        }
    }

    private static void OnLogin() {
        Task.Run(async () => {
            System.SystemConfig = await SystemConfig.Load();
            await System.ModuleManager.LoadModules();
        });
    }

    private static void OnLogout(int type, int code) {
        Task.Run(async () => {
            await System.ModuleManager.UnloadModules();
            System.SystemConfig = null;
        });
    }
}
