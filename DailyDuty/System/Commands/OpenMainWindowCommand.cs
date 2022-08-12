using DailyDuty.Interfaces;
using DailyDuty.UserInterface.Windows;

namespace DailyDuty.System.Commands;

internal class OpenMainWindowCommand : IPluginCommand
{
    public string? CommandArgument => null;

    private MainWindow? mainWindow;

    public bool CanExecute()
    {
        mainWindow = Service.System?.WindowManager.GetWindowOfType<MainWindow>();

        return mainWindow != null;
    }

    public void ExecuteInner(string? additionalArguments)
    {
        mainWindow!.IsOpen = true;
    }
}