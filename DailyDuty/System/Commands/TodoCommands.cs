using System.Collections.Generic;
using DailyDuty.System.Localization;
using KamiLib.ChatCommands;
using KamiLib.Interfaces;

namespace DailyDuty.System.Commands;

internal class TodoCommands : IPluginCommand
{
    public string CommandArgument => "todo";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = "show",
            Aliases = new List<string>{"enable"},
            CanExecute = () => Service.ClientState.IsLoggedIn,
            CommandAction = () =>
            {
                DailyDutyPlugin.System.TodoController.Config.Enable = true;
                DailyDutyPlugin.System.TodoController.SaveConfig();
            },
            GetHelpText = () => Strings.TodoEnable,
        },
        new SubCommand
        {
            CommandKeyword = "hide",
            Aliases = new List<string>{"disable"},
            CommandAction = () =>
            {
                DailyDutyPlugin.System.TodoController.Config.Enable = false;
                DailyDutyPlugin.System.TodoController.SaveConfig();
            },
            GetHelpText = () => Strings.TodoDisable,
        },
        new SubCommand
        {
            CommandKeyword = "toggle",
            Aliases = new List<string>{"t"},
            CommandAction = () =>
            {
                DailyDutyPlugin.System.TodoController.Config.Enable = !DailyDutyPlugin.System.TodoController.Config.Enable;
                DailyDutyPlugin.System.TodoController.SaveConfig();
            },
            GetHelpText = () => Strings.TodoToggle,
        }
    };
}