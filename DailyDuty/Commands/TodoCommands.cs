using System.Collections.Generic;
using DailyDuty.Localization;
using KamiLib.ChatCommands;
using KamiLib.Interfaces;

namespace DailyDuty.Commands;

internal class TodoCommands : IPluginCommand
{
    public string CommandArgument => "todo";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = "show",
            Aliases = new List<string>{"enable"},
            CommandAction = () =>
            {
                Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled.Value = true;
                Service.ConfigurationManager.Save();
            },
            GetHelpText = () => Strings.Commands_Todo_EnableOverlay,
        },
        new SubCommand
        {
            CommandKeyword = "hide",
            Aliases = new List<string>{"disable"},
            CommandAction = () =>
            {
                Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled.Value = false;
                Service.ConfigurationManager.Save();
            },
            GetHelpText = () => Strings.Commands_Todo_DisableOverlay,
        },
        new SubCommand
        {
            CommandKeyword = "toggle",
            Aliases = new List<string>{"t"},
            CommandAction = () =>
            {
                var value = Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled.Value;
                
                Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled.Value = !value;
                Service.ConfigurationManager.Save();
            },
            GetHelpText = () => Strings.Commands_Todo_ToggleOverlay,
        },
        new SubCommand
        {
            CommandKeyword = "repeat",
            CommandAction = () =>
            {
                Service.ChatManager.SendMessages();
            },
            GetHelpText = () => Strings.Commands_Todo_RepeatMessages
        }
    };
}