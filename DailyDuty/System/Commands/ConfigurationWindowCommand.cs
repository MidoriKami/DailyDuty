using DailyDuty.Interfaces;
using DailyDuty.UserInterface.Windows;
using DailyDuty.Utilities;

namespace DailyDuty.System.Commands;

internal class ConfigurationWindowCommand : IPluginCommand
{
    public string? CommandArgument => null;

    public void Execute(string? additionalArguments)
    {
        if ( Service.WindowManager.GetWindowOfType<ConfigurationWindow>() is {} mainWindow )
        {
            if (Service.ClientState.IsPvP)
            {
                Chat.PrintError("The configuration menu cannot be opened while in a PvP area");
            }
            else
            {
                mainWindow.IsOpen = !mainWindow.IsOpen;
            }
        }
    }
}