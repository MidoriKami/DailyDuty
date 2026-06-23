using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Utilities;
using DailyDuty.Windows;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using KamiToolKit;
using static DailyDuty.Utilities.Localization;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IAsyncDalamudPlugin {
    [PluginService] private static IDalamudPluginInterface PluginInterface { get; set; } = null!;

    public Task LoadAsync(CancellationToken cancellationToken) {
        PluginInterface.Create<Services>();

        KamiToolKitLibrary.Initialize(PluginInterface, "DailyDuty");
        KamiToolKitLibrary.SetResourceManager(Strings.ResourceManager);

        Localization.SetCultureInfo(PluginInterface.UiLanguage);
        PluginInterface.LanguageChanged += Localization.SetCultureInfo;

        System.ConfigurationWindow = new ModuleBrowserWindow {
            InternalName = "DailyDutyConfig",
            Title = Strings.Daily_Duty_Configuration,
            Size = new Vector2(700.0f, 600.0f),
        };

        Services.CommandManager.AddHandler("/dd", new CommandInfo(OnCommandReceived) {
            HelpMessage = Strings.DutyFinderEnhancements_OpenDailyDuty,
            ShowInHelp = true,
        });

        Services.CommandManager.AddHandler("/dailyduty", new CommandInfo(OnCommandReceived) {
            HelpMessage = Strings.DutyFinderEnhancements_OpenDailyDuty,
            ShowInHelp = true,
        });

        System.PayloadController = new PayloadController();
        System.ModuleManager = new ModuleManager();

        if (Services.ClientState.IsLoggedIn) {
            OnLogin();

            System.ModuleManager.OnLoadComplete += () => System.ConfigurationWindow.DebugOpen();
        }

        Services.ClientState.Login += OnLogin;
        Services.ClientState.Logout += OnLogout;

        Services.PluginInterface.UiBuilder.OpenConfigUi += System.ConfigurationWindow.Toggle;
        Services.PluginInterface.UiBuilder.OpenMainUi += System.ConfigurationWindow.Toggle;

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync() {
        Services.PluginInterface.UiBuilder.OpenConfigUi -= System.ConfigurationWindow.Toggle;
        Services.PluginInterface.UiBuilder.OpenMainUi -= System.ConfigurationWindow.Toggle;

        Services.ClientState.Login -= OnLogin;
        Services.ClientState.Logout -= OnLogout;

        Services.CommandManager.RemoveHandler("/dd");
        Services.CommandManager.RemoveHandler("/dailyduty");

        System.PayloadController.Dispose();

        if (!Services.Framework.IsFrameworkUnloading) {
            await System.ConfigurationWindow.DisposeAsync();
            await System.ModuleManager.DisposeAsync();
        }

        await Services.Framework.RunOnFrameworkThread(KamiToolKitLibrary.Dispose);
    }

    private static void OnCommandReceived(string command, string arguments) {
        if (command is not ("/dailyduty" or "/dd")) return;

        switch (arguments.Split(" ")) {
            case [ "" ] or [] or null:
                System.ConfigurationWindow.Toggle();
                break;

            case [ "logevents" ] when System.SystemConfig is not null:
                System.SystemConfig.EnableSceneEventLogging = !System.SystemConfig.EnableSceneEventLogging;
                var enabled = Strings.Enabled;
                var disabled = Strings.Disabled;
                var message = Strings.Event_logging_is_now;
                Services.ChatGui.Print($"{message} {(System.SystemConfig.EnableSceneEventLogging ? enabled : disabled)}", "DailyDuty");
                Services.PluginLog.Info($"Event is now {(System.SystemConfig.EnableSceneEventLogging ? "Enabled" : "Disabled")}");
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
