using DailyDuty.Interfaces;
using DailyDuty.UserInterface.Windows;

namespace DailyDuty.System.Commands;

internal class TimersWindowCommand : IPluginCommand
{
    public string CommandArgument => "timers";

    public void Execute(string? additionalArguments)
    {
        var configurationWindow = Service.WindowManager.GetWindowOfType<TimersConfigurationWindow>();
        var overlayWindow = Service.WindowManager.GetWindowOfType<TimersOverlayWindow>();

        if (configurationWindow == null || overlayWindow == null) return;

        switch (additionalArguments)
        {
            case null:
                configurationWindow.IsOpen = !configurationWindow.IsOpen;
                break;

            case "show":
                overlayWindow.IsOpen = true;
                break;

            case "hide":
                overlayWindow.IsOpen = false;
                break;

            case "toggle":
                overlayWindow.IsOpen = !overlayWindow.IsOpen;
                break;
        }
    }
}