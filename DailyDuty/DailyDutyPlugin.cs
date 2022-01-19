using DailyDuty.CommandSystem;
using DailyDuty.ConfigurationSystem;
using DailyDuty.DisplaySystem;
using DailyDuty.System;
using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Plugin;
using NotImplementedException = System.NotImplementedException;

namespace DailyDuty
{
    public sealed class DailyDutyPlugin : IDalamudPlugin
    {
        public string Name => "DailyDuty";

        private DisplayManager DisplayManager { get; init; }
        private CommandManager CommandManager { get; init; }
        private ModuleManager ModuleManager { get; init; }

        public DailyDutyPlugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();

            // If configuration json exists load it, if not make new config object
            Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Service.Configuration.Initialize(Service.PluginInterface);

            // Create Systems
            ModuleManager = new ModuleManager();
            DisplayManager = new DisplayManager(ModuleManager);
            CommandManager = new CommandManager(DisplayManager);

            // Register draw callbacks
            Service.PluginInterface.UiBuilder.Draw += DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            Service.Framework.Update += OnFrameworkUpdate;

            // Register Windows
            Service.WindowSystem.AddWindow(DisplayManager);

            Service.Chat.Enable();
        }

        private void OnFrameworkUpdate(Framework framework)
        {
            ModuleManager.Update();
        }

        private void DrawUI()
        {
            Service.WindowSystem.Draw();
        }
        private void DrawConfigUI()
        {
            DisplayManager.IsOpen = true;
        }

        public void Dispose()
        {
            CommandManager.Dispose();
            DisplayManager.Dispose();
            ModuleManager.Dispose();
        }
    }
}
