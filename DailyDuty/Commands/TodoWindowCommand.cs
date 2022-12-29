using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.UserInterface.Windows;
using KamiLib;
using KamiLib.CommandSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.Commands;

internal class TodoWindowCommand : IPluginCommand
{
    public string CommandArgument => "todo";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () => Chat.PrintError("The configuration window cannot be opened while in a PvP area"),
            CanExecute = () => Service.ClientState.IsPvP,
            GetHelpText = () => "Open Todo Configuration Window"
        },
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () =>
            {
                if ( KamiCommon.WindowManager.GetWindowOfType<TodoConfigurationWindow>() is {} mainWindow )
                {
                    Chat.Print("Command",!mainWindow.IsOpen ? "Opening Timers Configuration Window" : "Closing Timers Configuration Window");

                    mainWindow.IsOpen = !mainWindow.IsOpen;
                }
                else
                {
                    Chat.PrintError("Something went wrong trying to open Configuration Window");
                }
            },
            CanExecute = () => !Service.ClientState.IsPvP,
            GetHelpText = () => "Open Todo Configuration Window"
        },
        new SubCommand
        {
            CommandKeyword = "show",
            Aliases = new List<string>{"enable"},
            CommandAction = () =>
            {
                Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled.Value = true;
                Chat.Print("Command", "Enabling Todo Overlay");
            },
            GetHelpText = () => "Enable Todo Overlay",
        },
        new SubCommand
        {
            CommandKeyword = "hide",
            Aliases = new List<string>{"disable"},
            CommandAction = () =>
            {
                Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled.Value = false;
                Chat.Print("Command", "Disabling Todo Overlay");
            },
            GetHelpText = () => "Disable Todo Overlay",
        },
        new SubCommand
        {
            CommandKeyword = "toggle",
            Aliases = new List<string>{"t"},
            CommandAction = () =>
            {
                var value = Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled.Value;
                
                Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled.Value = !value;
                Chat.Print("Command", $"{(!value ? "Enabling" : "Disabling")} Todo Overlay");
            },
            GetHelpText = () => "Toggle Todo Overlay",
        },
    };
}