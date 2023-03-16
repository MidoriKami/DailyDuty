using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.DutyState;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace DailyDuty;

public sealed class Service
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] public static ChatGui Chat { get; set; } = null!;
    [PluginService] public static ClientState ClientState { get; set; } = null!;
    [PluginService] public static Framework Framework { get; set; } = null!;
    [PluginService] public static GameGui GameGui { get; set; } = null!;
    [PluginService] public static TargetManager TargetManager { get; set; } = null!;
    [PluginService] public static DutyState DutyState { get; set; } = null!;
    [PluginService] public static ToastGui Toast { get; set; } = null!;
}