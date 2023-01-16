using System.Collections.Generic;
using DailyDuty.Localization;
using KamiLib.ChatCommands;
using KamiLib.Interfaces;

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
                Service.ConfigurationManager.Save();
            },
            GetHelpText = () => Strings.Commands_Timers_EnableOverlay,
        },
        new SubCommand
        {
            CommandKeyword = "hide",
            Aliases = new List<string>{"disable"},
            CommandAction = () =>
            {
                Service.ConfigurationManager.CharacterConfiguration.TimersOverlay.Enabled.Value = false;
                Service.ConfigurationManager.Save();
            },
            GetHelpText = () => Strings.Commands_Timers_DisableOverlay,
        },
        new SubCommand
        {
            CommandKeyword = "toggle",
            Aliases = new List<string>{"t"},
            CommandAction = () =>
            {
                var value = Service.ConfigurationManager.CharacterConfiguration.TimersOverlay.Enabled.Value;
                
                Service.ConfigurationManager.CharacterConfiguration.TimersOverlay.Enabled.Value = !value;
                Service.ConfigurationManager.Save();
            },
            GetHelpText = () => Strings.Commands_Timers_ToggleOverlay,
        },
    };
}