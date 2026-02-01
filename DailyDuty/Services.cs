using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace DailyDuty;

public sealed class Services {
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] public static IChatGui ChatGui { get; set; } = null!;
    [PluginService] public static IClientState ClientState { get; set; } = null!;
    [PluginService] public static IFramework Framework { get; set; } = null!;
    [PluginService] public static IAddonLifecycle AddonLifecycle { get; set; } = null!;
    [PluginService] public static IPluginLog PluginLog { get; set; } = null!;
    [PluginService] public static ICondition Condition { get; set; } = null!;
    [PluginService] public static IGameInteropProvider Hooker { get; set; } = null!;
    [PluginService] public static IDataManager DataManager { get; set; } = null!;
    [PluginService] public static IGameInventory GameInventory { get; set; } = null!;
    [PluginService] public static IDutyState DutyState { get; set; } = null!;
    [PluginService] public static IDtrBar DtrBar { get; set; } = null!;
    [PluginService] public static IPlayerState PlayerState { get; set; } = null!;
    [PluginService] public static ICommandManager CommandManager { get; set; } = null!;
    [PluginService] public static IObjectTable ObjectTable { get; set; } = null!;
    [PluginService] public static IAgentLifecycle AgentLifecycle { get; set; } = null!;
}
