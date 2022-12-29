using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.IoC;
using Dalamud.Plugin;
using LocalizationManager = DailyDuty.System.LocalizationManager;

namespace DailyDuty;

internal class Service
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static ChatGui Chat { get; private set; } = null!;
    [PluginService] public static ClientState ClientState { get; private set; } = null!;
    [PluginService] public static Framework Framework { get; private set; } = null!;
    [PluginService] public static AetheryteList AetheryteList { get; private set;} = null!;
    [PluginService] public static ToastGui Toast { get; private set;} = null!;
    [PluginService] public static GameGui GameGui { get; private set; } = null!;
    [PluginService] public static TargetManager TargetManager { get; private set; } = null!;

    public static System.ModuleManager ModuleManager = null!;
    public static System.ConfigurationManager ConfigurationManager = null!;
    public static System.ChatManager ChatManager = null!;
    public static System.ResetManager ResetManager = null!;
    public static System.TeleportManager TeleportManager = null!;
    public static System.ChatPayloadManager PayloadManager = null!;
    public static System.AddonManager AddonManager = null!;
    public static LocalizationManager Localization = null!;
}