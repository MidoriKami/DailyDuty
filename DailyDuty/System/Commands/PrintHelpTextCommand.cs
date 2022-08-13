using DailyDuty.Interfaces;

namespace DailyDuty.System.Commands;

internal class PrintHelpTextCommand : IPluginCommand
{
    public string CommandArgument => "help";

    public bool CanExecute()
    {
        return true;
    }

    public void ExecuteInner(string? additionalArguments)
    {

    }
}