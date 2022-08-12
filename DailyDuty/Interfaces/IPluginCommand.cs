using DailyDuty.Utilities;

namespace DailyDuty.Interfaces;

internal interface IPluginCommand
{
    string? CommandArgument { get; }

    bool CanExecute();

    void Execute(string? additionalArguments)
    {
        Log.Verbose($"Attempting to execute command `{CommandArgument}` with additional arguments `{additionalArguments}`");

        if (CanExecute())
        {
            Log.Verbose("Command is executable, executing");

            ExecuteInner(additionalArguments);
        }
    }

    void ExecuteInner(string? additionalArguments);
}