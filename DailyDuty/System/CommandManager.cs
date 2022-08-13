using System;
using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.System.Commands;
using DailyDuty.Utilities;
using Dalamud.Game.Command;

namespace DailyDuty.System;

internal class CommandManager : IDisposable
{
    private const string SettingsCommand = "/dd";
    private const string HelpCommand = "/dd help";

    private readonly List<IPluginCommand> commands = new()
    {
        new OpenMainWindowCommand(),
        new PrintHelpTextCommand(),
    };

    public CommandManager()
    {
        Log.Verbose("Constructing");

        Log.Verbose($"Adding Command Handler: {SettingsCommand}");
        Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = "open configuration window"
        });

        Log.Verbose($"Adding Command Handler: {HelpCommand}");
        Service.Commands.AddHandler(HelpCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = "display a list of all available sub-commands"
        });
    }

    public void Dispose()
    {
        Service.Commands.RemoveHandler(SettingsCommand);
        Service.Commands.RemoveHandler(HelpCommand);
    }

    private void OnCommand(string command, string arguments)
    {
        Log.Verbose($"Received Command `{command}` `{arguments}`");

        var subCommand = GetPrimaryCommand(arguments);
        var subCommandArguments = GetSecondaryCommand(arguments);

        switch (subCommand)
        {
            case null:
                commands[0].Execute(subCommandArguments);
                break;

            case "help":
                commands[1].Execute(subCommandArguments);
                break;

            default:
                foreach (var cmd in commands)
                {
                    if (cmd.CommandArgument == subCommand)
                    {
                        cmd.Execute(subCommandArguments);
                    }
                }
                break;
        }
    }
    private static string? GetSecondaryCommand(string arguments)
    {
        var stringArray = arguments.Split(' ');

        if (stringArray.Length == 1)
        {
            return null;
        }

        return stringArray[1];
    }

    private static string? GetPrimaryCommand(string arguments)
    {
        var stringArray = arguments.Split(' ');

        if (stringArray[0] == string.Empty)
        {
            return null;
        }

        return stringArray[0];
    }
}