using System;
using System.IO;
using DailyDuty.System;
using DailyDuty.Utilities;
using DailyDuty.Windows.DailyDutyWindow;
using Dalamud.Game.Command;
using Dalamud.Logging;
using Dalamud.Plugin;
using Strings = DailyDuty.Localization.Strings;

namespace DailyDuty
{
    public sealed class DailyDutyPlugin : IDalamudPlugin
    {
        public string Name => "DailyDuty";
        private const string SettingsCommand = "/dd";
        private const string HelpCommand = "/dd help";

        public DailyDutyPlugin(DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();
            
            // Register Slash Commands
            Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            Service.Commands.AddHandler(HelpCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "display a list of all available sub-commands"
            });

            // Initialize Log Manager for Configuration
            Service.LogManager = new LogManager();

            // Load Configurations
            Configuration.Startup();

            // Initialize Languages
            var assemblyLocation = Service.PluginInterface.AssemblyLocation.DirectoryName!;
            var filePath = Path.Combine(assemblyLocation, @"translations");

            Service.Localization = new Dalamud.Localization(filePath, "DailyDuty_");
            if (Service.SystemConfiguration.System.SelectedLanguage != string.Empty)
            {
                LoadLocalization(Service.SystemConfiguration.System.SelectedLanguage);
            }
            else
            {
                LoadLocalization(pluginInterface.UiLanguage);
            }

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
            Service.PluginInterface.LanguageChanged += LoadLocalization;
        }

        public static void LoadLocalization(string languageCode)
        {
            try
            {
                PluginLog.Information($"Loading Localization for {languageCode}");

                Service.Localization.SetupWithLangCode(languageCode);

                Strings.Tabs = new Strings.TabStrings();
                Strings.Configuration = new Strings.ConfigurationStrings();
                Strings.Features = new Strings.FeaturesStrings();
                Strings.Common = new Strings.CommonStrings();
                Strings.Module = new Strings.ModuleStrings();
                Strings.Timers = new Strings.TimersStrings();
                Strings.Command = new Strings.CommandStrings();
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Unable to Load Localization");
            }
        }
        
        private void OnCommand(string command, string arguments)
        {
            Service.WindowManager.ExecuteCommand(command, arguments);
            Service.ModuleManager.ProcessCommand(command, arguments);

            if (arguments == Strings.Command.Help || arguments == "help")
            {
                Chat.Print(Strings.Command.Core, Strings.Command.HelpCommands);
            }
#if DEBUG
            else if (arguments.ToLower() == "generateloc" && Service.SystemConfiguration.DeveloperMode)
            {
                Chat.Debug("Generating Localization File");
                Service.Localization.ExportLocalizable();
            }
#else
            else if (arguments.ToLower() == "generateloc" && Service.SystemConfiguration.DeveloperMode)
            {
                Chat.Debug("Command not available in release mode");
            }
#endif
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
            Service.Commands.RemoveHandler(HelpCommand);

            Configuration.Cleanup();

            Service.PluginInterface.LanguageChanged -= LoadLocalization;
        }
    }
}