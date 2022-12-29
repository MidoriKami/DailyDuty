using System.Collections.Generic;
using DailyDuty.UserInterface.Windows;
using KamiLib;
using KamiLib.CommandSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.Commands;

internal class StatusWindowCommand : IPluginCommand
{
    public string CommandArgument => "status";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () => Chat.PrintError("The configuration window cannot be opened while in a PvP area"),
            CanExecute = () => Service.ClientState.IsPvP,
            GetHelpText = () => "Open Status Window"
        },
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () =>
            {
                if ( KamiCommon.WindowManager.GetWindowOfType<StatusWindow>() is {} mainWindow )
                {
                    Chat.Print("Command",!mainWindow.IsOpen ? "Opening Status Window" : "Closing Status Window");

                    mainWindow.IsOpen = !mainWindow.IsOpen;
                }
                else
                {
                    Chat.PrintError("Something went wrong trying to open Configuration Window");
                }
            },
            CanExecute = () => !Service.ClientState.IsPvP,
            GetHelpText = () => "Open Status Window"
        },
    };
}