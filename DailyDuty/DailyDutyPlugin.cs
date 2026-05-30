using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Windows;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using KamiToolKit;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IAsyncDalamudPlugin {
    [PluginService] private static IDalamudPluginInterface PluginInterface { get; set; } = null!;

    public Task LoadAsync(CancellationToken cancellationToken) {
        PluginInterface.Create<Services>();

        KamiToolKitLibrary.Initialize(PluginInterface, "DailyDuty");

        System.ConfigurationWindow = new ModuleBrowserWindow {
            InternalName = "DailyDutyConfig",
            Title = "Daily Duty Configuration",
            Size = new Vector2(700.0f, 600.0f),
        };

        Services.CommandManager.AddHandler("/dd", new CommandInfo(OnCommandReceived) {
            HelpMessage = "Open DailyDuty Config Window",
            ShowInHelp = true,
        });

        Services.CommandManager.AddHandler("/dailyduty", new CommandInfo(OnCommandReceived) {
            HelpMessage = "Open DailyDuty Config Window",
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

    private static void OnCommandReceived(string command, string arguments) {
        if (command is not ("/dailyduty" or "/dd")) return;

        switch (arguments) {
            case null or "":
                System.ConfigurationWindow.Toggle();
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

    public async ValueTask DisposeAsync() {
        Services.PluginInterface.UiBuilder.OpenConfigUi -= System.ConfigurationWindow.Toggle;
        Services.PluginInterface.UiBuilder.OpenMainUi -= System.ConfigurationWindow.Toggle;

        Services.ClientState.Login -= OnLogin;
        Services.ClientState.Logout -= OnLogout;

        Services.CommandManager.RemoveHandler("/dd");
        Services.CommandManager.RemoveHandler("/dailyduty");

        await System.ConfigurationWindow.DisposeAsync();

        System.PayloadController.Dispose();
        await System.ModuleManager.DisposeAsync();

        await Services.Framework.Run(KamiToolKitLibrary.Dispose);
    }
}
