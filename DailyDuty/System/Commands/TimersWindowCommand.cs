using DailyDuty.Interfaces;
using DailyDuty.UserInterface.Windows;

namespace DailyDuty.System.Commands;

internal class TimersWindowCommand : IPluginCommand
{
    public string CommandArgument => "timers";

    public void Execute(string? additionalArguments)
    {
        var configurationWindow = Service.WindowManager.GetWindowOfType<TimersConfigurationWindow>();
        var overlayWindow = Service.ConfigurationManager.CharacterConfiguration.TimersOverlay.Enabled;

        if (configurationWindow == null) return;

        switch (additionalArguments)
        {
            case null:
                configurationWindow.IsOpen = !configurationWindow.IsOpen;
                break;

            case "show":
                overlayWindow.Value = true;
                break;

            case "hide":
                overlayWindow.Value = false;
                break;

            case "toggle":
                overlayWindow.Value = !overlayWindow.Value;
                break;
        }

        Service.ConfigurationManager.Save();
    }
}