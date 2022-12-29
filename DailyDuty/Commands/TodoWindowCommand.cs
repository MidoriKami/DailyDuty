using DailyDuty.Interfaces;
using DailyDuty.UserInterface.Windows;

namespace DailyDuty.Commands;

internal class TodoWindowCommand : IPluginCommand
{
    public string CommandArgument => "todo";

    public void Execute(string? additionalArguments)
    {
        var configurationWindow = Service.WindowManager.GetWindowOfType<TodoConfigurationWindow>();
        var overlayWindow = Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled;

        if (configurationWindow == null) return;

        switch (additionalArguments)
        {
            case null:
                Service.ChatManager.SendMessages();
                break;

            case "config":
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

            default:
                IPluginCommand.PrintCommandError(CommandArgument, additionalArguments);
                break;
        }

        Service.ConfigurationManager.Save();
    }
}