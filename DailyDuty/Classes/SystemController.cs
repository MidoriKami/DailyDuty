using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using Dalamud.Excel.Sheets;
using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using KamiToolKit.Extensions;
using Lumina.Excel;

namespace DailyDuty.Classes;

public class SystemController : IAsyncDisposable {
    public List<IModule> Modules { get; private set; } = [];
    public List<IFeature> Features { get; private set; } = [];

    public List<IResettable> Resettables { get; } = [];
    public List<IToggleable> Toggleables { get; } = [];
    public List<IFrameworkUpdatable> Updatables { get; } = [];
    public List<ICommandListener> CommandListeners { get; } = [];
    public List<INpcInteractListener> NpcInteractListeners { get; } = [];

    public List<IModule> DailyModules { get; } = [];
    public List<IModule> WeeklyModules { get; } = [];
    public List<IModule> OtherModules { get; } = [];

    private SystemConfig? Config { get; set; }

    private Hook<EventFramework.Delegates.ProcessEventPlay>? frameworkEventHook;

    public async Task InitializeAsync() {
        Modules = ActivateAll<IModule>();
        Features = ActivateAll<IFeature>();

        foreach (var module in Modules) {
            RegisterInterfaces(module);

            switch (module) {
                case IWeeklyResettable:
                    WeeklyModules.Add(module);
                    break;

                case IDailyResettable:
                    DailyModules.Add(module);
                    break;

                case ISpecialResettable:
                    OtherModules.Add(module);
                    break;
            }
        }

        foreach (var feature in Features) {
            RegisterInterfaces(feature);
        }

        IFramework.Get().Update += OnFrameworkUpdate;
        IClientState.Get().TerritoryChanged += OnTerritoryChanged;

        IClientState.Get().Login += OnLogin;
        IClientState.Get().Logout += OnLogout;

        foreach (var command in new[] { "/dd", "/dailyduty" }) {
            ICommandManager.Get().AddHandler(command, new CommandInfo(OnCommandReceived) {
                HelpMessage = Strings.DutyFinderEnhancements_OpenDailyDuty,
                ShowInHelp = true,
            });
        }

        await IFramework.Get().Run(() => {
            unsafe {
                frameworkEventHook = IGameInteropProvider.Get()
                    .HookFromAddress<EventFramework.Delegates.ProcessEventPlay>(
                        EventFramework.MemberFunctionPointers.ProcessEventPlay,
                        OnProcessEventPlay
                    );

                frameworkEventHook.Enable();
            }
        });

        DailyDuty.PluginInterface.UiBuilder.OpenConfigUi += System.ConfigWindow.Toggle;
        DailyDuty.PluginInterface.UiBuilder.OpenMainUi += System.ConfigWindow.Toggle;

        if (IClientState.Get().IsLoggedIn) {
            await OnLoginAsync();
        }
    }

    public async ValueTask DisposeAsync() {
        DailyDuty.PluginInterface.UiBuilder.OpenConfigUi -= System.ConfigWindow.Toggle;
        DailyDuty.PluginInterface.UiBuilder.OpenMainUi -= System.ConfigWindow.Toggle;

        await IFramework.Get().Run(() => {
            frameworkEventHook?.Dispose();
            frameworkEventHook = null;
        });

        foreach (var command in new[] { "/dd", "/dailyduty" }) {
            ICommandManager.Get().RemoveHandler(command);
        }

        IClientState.Get().Logout -= OnLogout;
        IClientState.Get().Login -= OnLogin;

        IClientState.Get().TerritoryChanged -= OnTerritoryChanged;
        IFramework.Get().Update -= OnFrameworkUpdate;
    }

    private void RegisterInterfaces(object system) {
        if (system is IToggleable toggleable) Toggleables.Add(toggleable);
        if (system is IFrameworkUpdatable updatable) Updatables.Add(updatable);
        if (system is ICommandListener commandListener) CommandListeners.Add(commandListener);
        if (system is INpcInteractListener npcInteractListener) NpcInteractListeners.Add(npcInteractListener);
        if (system is IResettable resettable) Resettables.Add(resettable);
    }

    private void OnCommandReceived(string command, string arguments) {
        if (command is not ("/dailyduty" or "/dd")) return;

        switch (arguments.Split(" ")) {
            case [ "" ] or [] or null:
                System.ConfigWindow.Toggle();
                break;

            case ["debug"] when Config is not null:
                Config.IsDebugMode = !Config.IsDebugMode;
                IChatGui.Get().Print($"Debug mode is now {(Config.IsDebugMode ? "Enabled" : "Disabled")}", "DailyDuty");
                IPluginLog.Get().Info($"Debug mode is now {(Config.IsDebugMode ? "Enabled" : "Disabled")}");
                Config.Save();

                if (!System.ConfigWindow.IsOpen) {
                    System.ConfigWindow.Open();
                }
                break;

            default:
                var isHandled = false;
                foreach (var listener in CommandListeners) {
                    listener.OnCommand(arguments, ref isHandled);
                    if (isHandled) break;
                }
                break;
        }
    }

    private void OnLogin()
        => Task.Run(OnLoginAsync);

    public async Task OnLoginAsync() {
        Config = await SystemConfig.Load();

        await Task.WhenAll(Toggleables
            .Where(toggleable => Config.EnabledModules.Contains(toggleable.Name))
            .Select(module => EnableModule(module, false))
        );

        if (Config.IsDebugMode) {
            await System.ConfigWindow.OpenAsync();
        }
    }

    private void OnLogout(int type, int code)
        => Task.Run(OnLogoutAsync);

    public async Task OnLogoutAsync() {
        await Task.WhenAll(Toggleables
            .Where(toggleable => toggleable is { Enabled: true })
            .Select(toggleable => DisableModule(toggleable, false))
        );

        Config = null;
    }

    private void OnFrameworkUpdate(IFramework framework) {
        foreach (var resettable in Resettables) {
            if (resettable is IToggleable { Enabled: false }) continue;

            if (resettable.IsResetDue()) {
                if (resettable is INamed { Name: var name }) {
                    IPluginLog.Get().Info($"Resetting {name}");
                }

                resettable.Reset();
            }
        }

        foreach (var updatable in Updatables) {
            if (updatable is IToggleable { Enabled: false }) continue;

            updatable.Update();
        }
    }

    private void OnTerritoryChanged(RowRef<TerritoryType> obj) {
        foreach (var module in Modules) {
            if (module is IToggleable { Enabled: false }) continue;

            module.SendZoneChangeMessage();
        }
    }

    private unsafe void OnProcessEventPlay(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) {
        frameworkEventHook!.Original(thisPtr, gameObject, eventId, scene, sceneFlags, sceneData, sceneDataCount);

        try {
            if (Config?.EnableSceneEventLogging ?? false) {
                IPluginLog.Get().Debug(
                    $"[FrameworkEvent]\n" +
                    $"Scene: {scene}, Flags: {sceneFlags}, EventId: {eventId.ContentId}-{eventId.EntryId}-{eventId.Id} DataCount: {sceneDataCount}\n" +
                    string.Join("\n", Enumerable.Range(0, sceneDataCount).Select(index => $"[{index}] {sceneData[index]}"))
                );
            }

        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        }

        try {
            foreach (var listener in NpcInteractListeners) {
                if (listener is IToggleable { Enabled: false }) continue;

                try {
                    listener.OnNpcInteract(thisPtr, gameObject, eventId, scene, sceneFlags, sceneData, sceneDataCount);
                }
                catch (Exception e) {
                    IPluginLog.Get().Exception(e);
                }
            }
        }
        catch (Exception e) {
            IPluginLog.Get().Exception(e);
        }
    }

    public async Task EnableModule(IToggleable toggleable, bool addToList = true) {

        if (Config is null) {
            IPluginLog.Get().Error("Attempted to enable module with invalid system config file.");
            return;
        }

        // Module is already enabled.
        if (toggleable.LoadState is State.Enabled) {
            return;
        }

        // If it's already in an errored state, don't attempt to enable.
        if (toggleable is { LoadState: State.Errored }) {
            IPluginLog.Get().Error($"[{toggleable.Name}] Attempted to enable errored module");
            return;
        }

        try {

            IPluginLog.Get().Info($"Enabling {toggleable.Name}");

            await toggleable.EnableAsync();
            toggleable.LoadState = State.Enabled;
            toggleable.Enabled = true;

            if (toggleable is IModule module) {
                module.SendLoginMessage();
            }

            if (addToList) {
                Config.EnabledModules.Add(toggleable.Name);
                await Config.Save();
            }

            IPluginLog.Get().Info($"Successfully Enabled {toggleable.Name}");

        }
        catch (Exception e) {
            try {

                IPluginLog.Get().Error(e, $"Error while enabling {toggleable.Name}, attempting to disable");

                toggleable.LoadState = State.Errored;
                toggleable.LoadErrorMessage = Strings.ModuleManager_LoadFailed;
                toggleable.Enabled = false;

                await toggleable.DisableAsync();

                IPluginLog.Get().Information($"Successfully disabled erroring module {toggleable.Name}");

            }
            catch (Exception exception) {

                toggleable.LoadErrorMessage = Strings.ModuleManager_CriticalError;
                IPluginLog.Get().Error(exception, $"Critical Error while trying to unload erroring module: {toggleable.Name}");

            }
        }
    }

    public async Task DisableModule(IToggleable toggleable, bool removeFromList = true) {

        if (Config is null) {
            IPluginLog.Get().Error("Attempted to disable module with invalid system config file.");
            return;
        }

        // Module is already disabled.
        if (toggleable.LoadState is State.Disabled) {
            return;
        }

        // If it's already in an errored state, don't attempt to disable it again.
        if (toggleable.LoadState is State.Errored) {
            IPluginLog.Get().Error($"[{toggleable.Name}] Attempted to disable errored modification");
            return;
        }

        try {

            IPluginLog.Get().Info($"Disabling {toggleable.Name}");

            await toggleable.DisableAsync();
            toggleable.LoadState = State.Disabled;
            toggleable.Enabled = true;

            if (removeFromList) {
                Config.EnabledModules.Remove(toggleable.Name);
                await Config.Save();
            }

            IPluginLog.Get().Information($"Successfully Disabled {toggleable.Name}");
        }
        catch (Exception e) {

            toggleable.LoadState = State.Errored;
            toggleable.Enabled = false;
            IPluginLog.Get().Error(e, $"Failed to Disable {toggleable.Name}");

        }
    }

    private static List<T> ActivateAll<T>() where T : class => [
        .. Assembly.GetCallingAssembly().GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Where(type => typeof(T).IsAssignableFrom(type))
            .Select(type => (T?)Activator.CreateInstance(type))
            .OfType<T>(),
    ];
}
