using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using CheapLoc;
using DailyDuty.CommandSystem;
using DailyDuty.ConfigurationSystem;
using DailyDuty.DisplaySystem;
using DailyDuty.System;
using DailyDuty.System.Utilities;
using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace DailyDuty
{
    public sealed class DailyDutyPlugin : IDalamudPlugin
    {
        public string Name => "DailyDuty";

        private DisplayManager DisplayManager { get; init; }
        private CommandManager CommandManager { get; init; }
        private ModuleManager ModuleManager { get; init; }

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
            Service.Configuration.UpdateCharacter();

            Service.Chat.Enable();

            // Create Systems
            ModuleManager = new ModuleManager();
            DisplayManager = new DisplayManager(ModuleManager);
            CommandManager = new CommandManager(DisplayManager);

            // Register draw callbacks
            Service.PluginInterface.UiBuilder.DisableCutsceneUiHide = true;
            Service.PluginInterface.UiBuilder.DisableAutomaticUiHide = true;
            Service.PluginInterface.UiBuilder.Draw += DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            Service.Framework.Update += OnFrameworkUpdate;

            // Register Windows
            Service.WindowSystem.AddWindow(DisplayManager);
        }
        
        private void OnLogout(object? sender, EventArgs e)
        {
            Service.LoggedIn = false;
        }
        private void OnFrameworkUpdate(Framework framework)
        {
            ModuleManager.Update();

            Util.UpdateDelayed(stopwatch, TimeSpan.FromMilliseconds(100), UpdateSelectedCharacter);
        }

        private void UpdateSelectedCharacter()
        {
            if (Service.ClientState.LocalContentId != 0)
            {
                Service.LoggedIn = true;
                if (Service.ClientState.LocalContentId != Service.Configuration.CurrentCharacter)
                {
                    Service.Configuration.UpdateCharacter();
                }
            }
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

            Service.ClientState.Logout -= OnLogout;

            Service.PluginInterface.UiBuilder.Draw -= DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
            Service.Framework.Update -= OnFrameworkUpdate;
        }
    }
}
