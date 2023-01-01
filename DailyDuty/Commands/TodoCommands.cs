using System.Collections.Generic;
using KamiLib.CommandSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

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
                Chat.Print("Command", "Enabling Todo Overlay");
            },
            GetHelpText = () => "Enable Todo Overlay",
        },
        new SubCommand
        {
            CommandKeyword = "hide",
            Aliases = new List<string>{"disable"},
            CommandAction = () =>
            {
                Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled.Value = false;
                Chat.Print("Command", "Disabling Todo Overlay");
            },
            GetHelpText = () => "Disable Todo Overlay",
        },
        new SubCommand
        {
            CommandKeyword = "toggle",
            Aliases = new List<string>{"t"},
            CommandAction = () =>
            {
                var value = Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled.Value;
                
                Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled.Value = !value;
                Chat.Print("Command", $"{(!value ? "Enabling" : "Disabling")} Todo Overlay");
            },
            GetHelpText = () => "Toggle Todo Overlay",
        },
        new SubCommand
        {
            CommandKeyword = "repeat",
            CommandAction = () =>
            {
                Service.ChatManager.SendMessages();
            },
            GetHelpText = () => "Re-display todo tasks in chat"
        }
    };
}