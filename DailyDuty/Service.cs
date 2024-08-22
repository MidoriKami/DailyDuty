using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace DailyDuty;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public sealed class Service {
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; set; }
    [PluginService] public static IChatGui Chat { get; set; }
    [PluginService] public static IClientState ClientState { get; set; }
    [PluginService] public static IFramework Framework { get; set; }
    [PluginService] public static IGameGui GameGui { get; set; }
    [PluginService] public static ITargetManager TargetManager { get; set; }
    [PluginService] public static IDutyState DutyState { get; set; }
    [PluginService] public static IToastGui Toast { get; set; }
    [PluginService] public static IObjectTable ObjectTable { get; set; }
    [PluginService] public static IAddonLifecycle AddonLifecycle { get; set; }
    [PluginService] public static IPluginLog Log { get; set; }
    [PluginService] public static ICondition Condition { get; set; }
    [PluginService] public static IGameInteropProvider Hooker { get; set; }
    [PluginService] public static IDataManager DataManager { get; set; }
    [PluginService] public static IAddonEventManager AddonEventManager { get; set; }
    [PluginService] public static IGameInventory GameInventory { get; set; }
    [PluginService] public static INotificationManager NotificationManager { get; set; }
}