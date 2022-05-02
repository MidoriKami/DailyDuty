using DailyDuty.Data;
using DailyDuty.System;
using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace DailyDuty
{
    public class Service
    {
        [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] public static ChatGui Chat { get; private set; } = null!;
        [PluginService] public static ClientState ClientState { get; private set; } = null!;
        [PluginService] public static PartyList PartyList { get; private set; } = null!;
        [PluginService] public static CommandManager Commands { get; private set; } = null!;
        [PluginService] public static Condition Condition { get; private set; } = null!;
        [PluginService] public static DataManager DataManager { get; private set; } = null!;
        [PluginService] public static Framework Framework { get; private set; } = null!;
        [PluginService] public static ObjectTable ObjectTable { get; private set; } = null!;
        [PluginService] public static GameGui GameGui { get; private set; } = null!;
        [PluginService] public static AetheryteList AetheryteList { get; private set;} = null!;
        [PluginService] public static ToastGui Toast { get; private set; } = null!;
        [PluginService] public static TargetManager TargetManager { get; private set; } = null!;

        public static WindowSystem WindowSystem { get; } = new("DailyDuty");
        public static WindowManager WindowManager { get; set; } = null!;
        public static AddonManager AddonManager { get; set; } = null!;
        public static LogManager LogManager { get; set; } = null!;
        public static ModuleManager ModuleManager { get; set; } = null!;
        public static TimerManager TimerManager { get; set; } = null!;
        public static TeleportManager TeleportManager { get; set; } = null!;
        public static SystemConfiguration SystemConfiguration { get; set; } = null!;
        public static CharacterConfiguration CharacterConfiguration { get; set; } = null!;
        public static Dalamud.Localization Localization { get; set; } = null!;

        public static bool LoggedIn = false;
    }
}