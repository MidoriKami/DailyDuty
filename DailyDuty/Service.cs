using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace DailyDuty;

public sealed class Service
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] public static ChatGui Chat { get; set; } = null!;
    [PluginService] public static IClientState ClientState { get; set; } = null!;
    [PluginService] public static Framework Framework { get; set; } = null!;
    [PluginService] public static IGameGui GameGui { get; set; } = null!;
    [PluginService] public static ITargetManager TargetManager { get; set; } = null!;
    [PluginService] public static IDutyState DutyState { get; set; } = null!;
    [PluginService] public static ToastGui Toast { get; set; } = null!;
    [PluginService] public static IObjectTable ObjectTable { get; set; } = null!;
    [PluginService] public static IAddonLifecycle AddonLifecycle { get; set; } = null!;
}