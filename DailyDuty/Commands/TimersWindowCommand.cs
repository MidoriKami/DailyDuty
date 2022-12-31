using System.Collections.Generic;
using DailyDuty.UserInterface.Windows;
using KamiLib;
using KamiLib.CommandSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.Commands;

internal class TimersWindowCommand : IPluginCommand
{
    public string CommandArgument => "timers";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () => Chat.PrintError("The timers window cannot be opened while in a PvP area"),
            CanExecute = () => Service.ClientState.IsPvP,
            GetHelpText = () => "Open Timers Configuration Window"
        },
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () =>
            {
                if ( KamiCommon.WindowManager.GetWindowOfType<TimersConfigurationWindow>() is {} mainWindow )
                {
                    Chat.Print("Command",!mainWindow.IsOpen ? "Opening Status Configuration Window" : "Closing Status Configuration Window");

                    mainWindow.IsOpen = !mainWindow.IsOpen;
                }
                else
                {
                    Chat.PrintError("Something went wrong trying to open Timers Window");
                }
            },
            CanExecute = () => !Service.ClientState.IsPvP,
            GetHelpText = () => "Open Timers Configuration Window"
        },
        new SubCommand
        {
            CommandKeyword = "show",
            Aliases = new List<string>{"enable"},
            CommandAction = () =>
            {
                Service.ConfigurationManager.CharacterConfiguration.TimersOverlay.Enabled.Value = true;
                Chat.Print("Command", "Enabling Timers Overlay");
            },
            GetHelpText = () => "Enable Timers Overlay",
        },
        new SubCommand
        {
            CommandKeyword = "hide",
            Aliases = new List<string>{"disable"},
            CommandAction = () =>
            {
                Service.ConfigurationManager.CharacterConfiguration.TimersOverlay.Enabled.Value = false;
                Chat.Print("Command", "Disabling Timers Overlay");
            },
            GetHelpText = () => "Disable Timers Overlay",
        },
        new SubCommand
        {
            CommandKeyword = "toggle",
            Aliases = new List<string>{"t"},
            CommandAction = () =>
            {
                var value = Service.ConfigurationManager.CharacterConfiguration.TimersOverlay.Enabled.Value;
                
                Service.ConfigurationManager.CharacterConfiguration.TimersOverlay.Enabled.Value = !value;
                Chat.Print("Command", $"{(!value ? "Enabling" : "Disabling")} Timers Overlay");
            },
            GetHelpText = () => "Toggle Timers Overlay",
        },
    };
}