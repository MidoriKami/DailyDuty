using System;
using System.Diagnostics;
using DailyDuty.Data;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IDalamudPlugin
{
    public string Name => "DailyDuty";
    private const string SettingsCommand = "/dailyduty";
    private const string ShorthandCommand = "/dd";

    private readonly Stopwatch stopwatch = new();

    public DailyDutyPlugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
    {
        // Create Static Services for use everywhere
        pluginInterface.Create<Service>();
            
        // If configuration json exists load it, if not make new config object
        Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Service.Configuration.Initialize(Service.PluginInterface);
        Service.ClientState.Logout += OnLogout;

        Service.Chat.Enable();

        // Register Slash Commands
        Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = "open configuration window"
        });

        Service.Commands.AddHandler(ShorthandCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = "shorthand command to open configuration window"
        });

        // Create Systems
        Service.TeleportManager = new();
        Service.ModuleManager = new();
        Service.WindowManager = new();
        Service.AddonManager = new();

        // Register draw callbacks
        Service.PluginInterface.UiBuilder.Draw += DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        Service.Framework.Update += OnFrameworkUpdate;
    }

    private void OnCommand(string command, string arguments)
    {
        if (arguments == string.Empty)
        {
            Service.WindowManager.ToggleSettingsWindow();
        }
        else
        {
            Service.ModuleManager.ProcessCommand(command, arguments);
        }
    }

    private void OnLogout(object? sender, EventArgs e)
    {
        ConfigurationHelper.Logout();
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        Time.UpdateDelayed(stopwatch, TimeSpan.FromMilliseconds(100), UpdateSelectedCharacter);
    }

    private void UpdateSelectedCharacter()
    {
        // If content id is valid and we aren't already logged in
        if (Service.ClientState.LocalContentId != 0 && Service.LoggedIn == false)
        {
            // login
            ConfigurationHelper.Login();
        }
    }

    private void DrawUI()
    {
        Service.WindowSystem.Draw();
    }

    private void DrawConfigUI()
    {
        Service.WindowManager.ToggleSettingsWindow();
    }

    public void Dispose()
    {
        Service.AddonManager.Dispose();
        Service.WindowManager.Dispose();
        Service.ModuleManager.Dispose();
        Service.TeleportManager.Dispose();

        Service.ClientState.Logout -= OnLogout;

        Service.PluginInterface.UiBuilder.Draw -= DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
        Service.Framework.Update -= OnFrameworkUpdate;

        Service.Commands.RemoveHandler(SettingsCommand);
        Service.Commands.RemoveHandler(ShorthandCommand);
    }
}