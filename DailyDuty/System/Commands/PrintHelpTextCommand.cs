using DailyDuty.Interfaces;
using DailyDuty.System.Localization;
using DailyDuty.Utilities;

namespace DailyDuty.System.Commands;

internal class PrintHelpTextCommand : IPluginCommand
{
    public string CommandArgument => "help";

    public void Execute(string? additionalArguments)
    {
        switch (additionalArguments)
        {
            case null:
                Chat.Print(Strings.Common.Command ,Strings.Command.Help.Base);
                break;

            case "timers":
                Chat.Print(Strings.Common.Command, Strings.Command.Help.Timers);
                break;

            case "todo":
                Chat.Print(Strings.Common.Command, Strings.Command.Help.Todo);
                break;
        }
    }
}