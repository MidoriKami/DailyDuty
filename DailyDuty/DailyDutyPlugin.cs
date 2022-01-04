using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using PartnerUp;
using System.IO;
using System.Reflection;

namespace PartnerUp
{
    public sealed class PartnerUpPlugin : IDalamudPlugin
    {
        public string Name => "Partner Up";

        private const string SettingsCommand = "/partnerup";

        private readonly SettingsWindow SettingsWindow;

        public PartnerUpPlugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();

            // If configuration json exists load it, if not make new config object
            Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Service.Configuration.Initialize(Service.PluginInterface);

            // Load Tank Stance warning image
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var dancePartnerPath = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, @"images\DancePartner.png");
            var faeriePath = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, @"images\Faerie.png");
            var kardionPath = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, @"images\Kardion.png");

            var dancePartnerImage = Service.PluginInterface.UiBuilder.LoadImage(dancePartnerPath);
            var faerieImage = Service.PluginInterface.UiBuilder.LoadImage(faeriePath);
            var kardionImage = Service.PluginInterface.UiBuilder.LoadImage(kardionPath);

            // Create Windows
            SettingsWindow = new SettingsWindow();

            // Register FrameworkUpdate
            Service.Framework.Update += OnFrameworkUpdate;

            // Register Slash Commands
            Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            // Register draw callbacks
            Service.PluginInterface.UiBuilder.Draw += DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            // Register Windows
            Service.WindowSystem.AddWindow(SettingsWindow);

            Service.Chat.Enable();
        }
        private void OnFrameworkUpdate(Framework framework)
        {
        }

        private void DrawUI()
        {
            Service.WindowSystem.Draw();
        }
        private void DrawConfigUI()
        {
            SettingsWindow.IsOpen = true;
        }

        private void OnCommand(string command, string arguments)
        {
            switch (arguments)
            {
                default:
                    break;
            }

            Service.Configuration.Save();
        }

        public void Dispose()
        {
            Service.Commands.RemoveHandler(SettingsCommand);
            Service.Framework.Update -= OnFrameworkUpdate;
            SettingsWindow.Dispose();
        }
    }
}
