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
                Chat.Print(Strings.Common_Command, Strings.Commands_Todo_EnablingOverlay);
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
                Chat.Print(Strings.Common_Command, Strings.Commands_Todo_DisablingOverlay);
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
                var enableDisable = !value ? Strings.Commands_Todo_EnablingOverlay : Strings.Commands_Todo_DisablingOverlay;
                
                Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled.Value = !value;
                Chat.Print(Strings.Common_Command, enableDisable);
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