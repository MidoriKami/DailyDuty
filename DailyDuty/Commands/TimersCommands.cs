using System.Collections.Generic;
using KamiLib.CommandSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.Commands;

internal class TimersCommands : IPluginCommand
{
    public string CommandArgument => "timers";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = "show",
            Aliases = new List<string>{"enable"},
            CommandAction = () =>
            {
                Service.ConfigurationManager.CharacterConfiguration.TimersOverlay.Enabled.Value = true;
                Chat.Print("Command", "Enabling Timers Overlay");
            },
            GetHelpText = () => "Enable Timers Overlay",
        },
        new SubCommand
        {
            CommandKeyword = "hide",
            Aliases = new List<string>{"disable"},
            CommandAction = () =>
            {
                Service.ConfigurationManager.CharacterConfiguration.TimersOverlay.Enabled.Value = false;
                Chat.Print("Command", "Disabling Timers Overlay");
            },
            GetHelpText = () => "Disable Timers Overlay",
        },
        new SubCommand
        {
            CommandKeyword = "toggle",
            Aliases = new List<string>{"t"},
            CommandAction = () =>
            {
                var value = Service.ConfigurationManager.CharacterConfiguration.TimersOverlay.Enabled.Value;
                
                Service.ConfigurationManager.CharacterConfiguration.TimersOverlay.Enabled.Value = !value;
                Chat.Print("Command", $"{(!value ? "Enabling" : "Disabling")} Timers Overlay");
            },
            GetHelpText = () => "Toggle Timers Overlay",
        },
    };
}