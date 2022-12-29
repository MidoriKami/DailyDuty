using DailyDuty.Interfaces;
using DailyDuty.UserInterface.Windows;
using KamiLib.Utilities;

namespace DailyDuty.System.Commands;

internal class StatusWindowCommand : IPluginCommand
{
    public string CommandArgument => "status";

    public void Execute(string? additionalArguments)
    {
        if ( Service.WindowManager.GetWindowOfType<StatusWindow>() is {} statusWindow )
        {
            if (Service.ClientState.IsPvP)
            {
                Chat.PrintError("The configuration menu cannot be opened while in a PvP area");
            }
            else
            {
                statusWindow.IsOpen = !statusWindow.IsOpen;
            }
        }
    }
}