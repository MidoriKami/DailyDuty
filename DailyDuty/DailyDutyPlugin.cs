using DailyDuty.Reminders;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace DailyDuty
{
    public sealed class DailyDutyPlugin : IDalamudPlugin
    {
        public string Name => "DailyDuty";

        private PluginWindow PluginWindow { get; init; }
        private CommandSystem.CommandSystem CommandSystem { get; init; }

        public DailyDutyPlugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();

            // If configuration json exists load it, if not make new config object
            Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Service.Configuration.Initialize(Service.PluginInterface);

            // Create Windows
            PluginWindow = new PluginWindow();

            // Register Slash Commands
            CommandSystem = new CommandSystem.CommandSystem(PluginWindow);

            // Register draw callbacks
            Service.PluginInterface.UiBuilder.Draw += DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            // Register Windows
            Service.WindowSystem.AddWindow(PluginWindow);

            Service.Chat.Enable();
        }

        private void DrawUI()
        {
            Service.WindowSystem.Draw();
        }
        private void DrawConfigUI()
        {
            PluginWindow.IsOpen = true;
        }

        public void Dispose()
        {
            CommandSystem.Dispose();
            PluginWindow.Dispose();
        }
    }
}
