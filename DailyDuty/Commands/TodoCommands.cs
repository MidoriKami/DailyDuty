using System.Collections.Generic;
using DailyDuty.Localization;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Common.Lua;
using KamiLib.CommandSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;
using Lumina.Data.Parsing;

namespace DailyDuty.Commands;

internal class TodoCommands : IPluginCommand
{
    public string CommandArgument => "todo";

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
                Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled.Value = true;
                Chat.Print(Strings.Common_Command, Strings.Commands_Todo_EnablingDisabling.Format(Strings.Common_Enabling));
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
                Chat.Print(Strings.Common_Command, Strings.Commands_Todo_EnablingDisabling.Format(Strings.Common_Disabling));
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
                var enableDisable = !value ? Strings.Common_Enabling : Strings.Common_Disabling;
                
                Service.ConfigurationManager.CharacterConfiguration.TodoOverlay.Enabled.Value = !value;
                Chat.Print(Strings.Common_Command, string.Format(Strings.Commands_Todo_EnablingDisabling, enableDisable));
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