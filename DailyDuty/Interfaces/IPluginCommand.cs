using DailyDuty.Localization;
using DailyDuty.Utilities;

namespace DailyDuty.Interfaces;

internal interface IPluginCommand
{
    string? CommandArgument { get; }

    void Execute(string? additionalArguments);

    static void PrintCommandError(string command, string? arguments)
    {
        Chat.PrintError(arguments != null
            ? $"{Strings.Command.InvalidCommand} `/dd {command} {arguments}`"
            : $"{Strings.Command.InvalidCommand} `/dd {command}`");
    }
}