using System;
using CheapLoc;
using DailyDuty.System;
using DailyDuty.Utilities;
using DailyDuty.Windows.DailyDutyWindow;
using Dalamud.Game.Command;
using Dalamud.Logging;
using Dalamud.Plugin;

namespace DailyDuty
{
    public sealed class DailyDutyPlugin : IDalamudPlugin
    {
        public string Name => "DailyDuty";
        private const string SettingsCommand = "/dailyduty";
        private const string ShorthandCommand = "/dd";

        public DailyDutyPlugin(DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();
            Service.Chat.Enable();

            Loc.SetupWithFallbacks();

            //try
            //{
            //    Loc.ExportLocalizable();
            //}
            //catch (Exception e)
            //{
            //    PluginLog.Error(e.Message);
            //    throw;
            //}

            // Register Slash Commands
            Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            Service.Commands.AddHandler(ShorthandCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "shorthand command to open configuration window"
            });


            // Initialize Log Manager for Configuration
            Service.LogManager = new LogManager();

            // Load Configurations
            Configuration.Startup();

            // Create Custom Services
            Service.TeleportManager = new TeleportManager();
            Service.TimerManager = new TimerManager();
            Service.ModuleManager = new ModuleManager();
            Service.WindowManager = new WindowManager();
            Service.AddonManager = new AddonManager();

            // Register draw callbacks
            Service.PluginInterface.UiBuilder.Draw += DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            Service.ClientState.Login += Configuration.Login;
            Service.ClientState.Logout += Configuration.Logout;
        }

        private void OnCommand(string command, string arguments)
        {
            Service.WindowManager.ExecuteCommand(command, arguments);
            Service.ModuleManager.ProcessCommand(command, arguments);
        }

        private void DrawUI() => Service.WindowSystem.Draw();

        private void DrawConfigUI()
        {
            var window = Service.WindowManager.GetWindowOfType<DailyDutyWindow>();

            if (window != null)
            {
                window.IsOpen = true;
            }
        }

        public void Dispose()
        {
            Service.TeleportManager.Dispose();
            Service.WindowManager.Dispose();
            Service.LogManager.Dispose();
            Service.AddonManager.Dispose();
            Service.ModuleManager.Dispose();
            Service.TimerManager.Dispose();

            Service.ClientState.Login -= Configuration.Login;
            Service.ClientState.Logout -= Configuration.Logout;

            Service.PluginInterface.UiBuilder.Draw -= DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

            Service.Commands.RemoveHandler(SettingsCommand);
            Service.Commands.RemoveHandler(ShorthandCommand);

            Configuration.Cleanup();
        }
    }
}