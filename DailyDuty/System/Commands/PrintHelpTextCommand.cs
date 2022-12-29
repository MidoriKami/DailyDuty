using DailyDuty.Interfaces;
using DailyDuty.Localization;
using KamiLib.Utilities;

namespace DailyDuty.System.Commands;

internal class PrintHelpTextCommand : IPluginCommand
{
    public string CommandArgument => "help";

    public void Execute(string? additionalArguments)
    {
        switch (additionalArguments)
        {
            case null:
                foreach (var message in Strings.Command.Help.CoreMessages)
                {
                    Chat.Print(Strings.Common.Command, message);
                }
                break;

            case "timers":
                foreach (var message in Strings.Command.Help.TimersMessages)
                {
                    Chat.Print(Strings.Common.Command, message);
                }
                break;

            case "todo":
                foreach (var message in Strings.Command.Help.TodoMessages)
                {
                    Chat.Print(Strings.Common.Command, message);
                }
                break;

            default:
                IPluginCommand.PrintCommandError(CommandArgument, additionalArguments);
                break;
        }
    }
}