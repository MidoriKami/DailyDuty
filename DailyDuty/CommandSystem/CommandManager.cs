using System;
using System.Collections.Generic;
using DailyDuty.DisplaySystem;
using Dalamud.Game.Command;

namespace DailyDuty.CommandSystem
{
    internal class CommandManager : IDisposable
    {
        private const string SettingsCommand = "/dailyduty";
        private const string ShorthandCommand = "/dd";

        private readonly DisplayManager displayManager;

        private readonly List<CommandProcessor> commandProcessors = new()
        {
        };

        public CommandManager(DisplayManager displayManager)
        {
            RegisterCommands();

            this.displayManager = displayManager;
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
            var primaryCommand = GetPrimaryCommand(arguments)?.ToLower();
            var secondaryCommand = GetSecondaryCommand(arguments)?.ToLower();

            switch (primaryCommand)
            {
                case null:
                    displayManager.IsOpen = !displayManager.IsOpen;
                    break;

                case "test":
                    break;

                default:
                    foreach (var commandProcessor in commandProcessors)
                    {
                        commandProcessor.ProcessCommand(primaryCommand, secondaryCommand);
                    }
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
