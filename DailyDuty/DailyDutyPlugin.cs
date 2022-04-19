using System.Linq;
using DailyDuty.Utilities;
using Dalamud.Game.Command;
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

            Configuration.Startup();

            // Register Slash Commands
            Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            Service.Commands.AddHandler(ShorthandCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "shorthand command to open configuration window"
            });

            // Register draw callbacks
            Service.PluginInterface.UiBuilder.Draw += DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            Service.ClientState.Login += Configuration.Login;
            Service.ClientState.Logout += Configuration.Logout;
        }

        private void OnCommand(string command, string arguments) => Service.DailyDuty.ExecuteCommand(command, arguments);

        private void DrawUI() => Service.WindowSystem.Draw();

        private void DrawConfigUI() => Service.WindowSystem.Windows.First(window => window.WindowName == "DailyDuty Window").IsOpen = true;

        public void Dispose()
        {
            Service.ClientState.Login -= Configuration.Login;
            Service.ClientState.Logout -= Configuration.Logout;

            Service.PluginInterface.UiBuilder.Draw -= DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

            Service.Commands.RemoveHandler(SettingsCommand);
            Service.Commands.RemoveHandler(ShorthandCommand);

            Configuration.Cleanup();

            Service.CharacterConfiguration.Save();
        }
    }
}