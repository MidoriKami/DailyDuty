using System.Collections.Generic;
using DailyDuty.Localization;
using Dalamud.Utility;
using KamiLib.CommandSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.Commands;

internal class TimersCommands : IPluginCommand
{
    public string CommandArgument => "timers";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand // window open command handled by KamiLib
        {
            CommandKeyword = null,
            Hidden = true,
        },
        new SubCommand
        {
            CommandKeyword = "show",
            Aliases = new List<string>{"enable"},
            CommandAction = () =>
            {
                Service.ConfigurationManager.CharacterConfiguration.TimersOverlay.Enabled.Value = true;
                Chat.Print(Strings.Common_Command, Strings.Commands_Timers_EnablingDisabling.Format(Strings.Common_Enabling));
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
                Chat.Print(Strings.Common_Command, Strings.Commands_Timers_EnablingDisabling.Format(Strings.Common_Disabling));
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
                var enablingDisabling = !value ? Strings.Common_Enabling : Strings.Common_Disabling;
                
                Service.ConfigurationManager.CharacterConfiguration.TimersOverlay.Enabled.Value = !value;
                Chat.Print(Strings.Common_Command, string.Format(Strings.Commands_Timers_EnablingDisabling, enablingDisabling));
            },
            GetHelpText = () => Strings.Commands_Timers_ToggleOverlay,
        },
    };
}