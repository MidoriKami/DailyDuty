using System;
using DailyDuty.Reminders;
using Dalamud.Game.Command;

namespace DailyDuty.CommandSystem
{
    internal class CommandSystem : IDisposable
    {

        private const string SettingsCommand = "/dailyduty";
        private const string ShorthandCommand = "/dd";

        private readonly PluginWindow pluginWindow;

        public CommandSystem(PluginWindow pluginWindow)
        {
            RegisterCommands();

            this.pluginWindow = pluginWindow;
        }

        private void RegisterCommands()
        {
            // Register Slash Commands
            Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            Service.Commands.AddHandler(ShorthandCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "shorthand command to open configuration window"
            });
        }

        // Valid Command Structure:
        // /nty [main command] [on/off/nothing]
        private void OnCommand(string command, string arguments)
        {
            var primaryCommand = GetPrimaryCommand(arguments);
            var secondaryCommand = GetSecondaryCommand(arguments);

            switch (primaryCommand?.ToLower())
            {
                case null:
                    pluginWindow.IsOpen = !pluginWindow.IsOpen;
                    break;

                default:
                    break;
            }

            Service.Configuration.Save();
        }

        private static string? GetSecondaryCommand(string arguments)
        {
            var stringArray = arguments.Split(' ');

            if (stringArray.Length == 1)
            {
                return null;
            }

            return stringArray[1];
        }

        private static string? GetPrimaryCommand(string arguments)
        {
            var stringArray = arguments.Split(' ');

            if (stringArray[0] == string.Empty)
            {
                return null;
            }

            return stringArray[0];
        }

        public void Dispose()
        {
            Service.Commands.RemoveHandler(SettingsCommand);
            Service.Commands.RemoveHandler(ShorthandCommand);
        }
    }
}
