using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DailyDuty.Enums;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace DailyDuty.Classes;

public class ModuleManager : IAsyncDisposable {

    public List<LoadedModule>? LoadedModules { get; private set; }
    private FrozenDictionary<string, LoadedModule>? loadedModulesByName;
    public bool IsUnloading { get; private set; }

    private Hook<EventFramework.Delegates.ProcessEventPlay>? frameworkEventHook;

    public Action? OnFeatureEnabled { get; set; }
    public Action? OnFeatureDisabled { get; set; }

    public unsafe ModuleManager() {
        frameworkEventHook = Services.Hooker.HookFromAddress<EventFramework.Delegates.ProcessEventPlay>(EventFramework.MemberFunctionPointers.ProcessEventPlay, OnFrameworkEvent);
    }

    public async ValueTask DisposeAsync() {
        frameworkEventHook?.Dispose();
        frameworkEventHook = null;

        await UnloadModules();
    }

    public async Task LoadModules() {
        frameworkEventHook?.Enable();

        IsUnloading = false;

        LoadedModules = [];
        List<Task> loadTasks = [];
        List<Task> enableTasks = [];

        var orderedModules = GetAllModules()
            .OrderBy(module => module.ModuleInfo.Type)
            .ThenBy(module => module.Name);

        // Load all modules before enabling any of them, so timers and others don't have to rebuild once load is done.
        foreach (var module in orderedModules) {
            Services.PluginInterface.Inject(module);

            var newLoadedModule = new LoadedModule(module, LoadedState.Disabled);
            LoadedModules.Add(newLoadedModule);

            loadTasks.Add(module.LoadAsync());
        }

        await Task.WhenAll(loadTasks);

        // Then enable modules that want to be enabled.
        foreach (var loadedModule in LoadedModules) {
            if (System.SystemConfig?.EnabledModules.Contains(loadedModule.Name) ?? false) {
                enableTasks.Add(TryEnableModule(loadedModule));
            }
        }

        await Task.WhenAll(enableTasks);

        loadedModulesByName = LoadedModules.ToFrozenDictionary(module => module.Name, module => module);

        await Services.Framework.RunSafely(() => System.ConfigurationWindow.DebugOpen());
    }

    private unsafe void OnFrameworkEvent(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) {
        frameworkEventHook!.Original(thisPtr, gameObject, eventId, scene, sceneFlags, sceneData, sceneDataCount);

        try {
            if (System.SystemConfig?.EnableSceneEventLogging ?? false) {
                Services.PluginLog.Debug($"[FrameworkEvent]\n" +
                                         $"Scene: {scene}, Flags: {sceneFlags}, EventId: {eventId.ContentId}-{eventId.EntryId}-{eventId.Id} DataCount: {sceneDataCount}\n" +
                                         string.Join("\n", Enumerable.Range(0, sceneDataCount).Select(index => $"[{index}] {sceneData[index]}")));
            }

            foreach (var loadedModule in LoadedModules ?? []) {
                if (loadedModule.FeatureBase is not ModuleBase module) continue;

                try {
                    module.OnNpcInteract(thisPtr, gameObject, eventId, scene, sceneFlags, sceneData, sceneDataCount);
                }
                catch (Exception e) {
                    Services.PluginLog.Error(e, $"Exception processing OnNpcInteract for {loadedModule.Name}");
                }
            }
        }
        catch (Exception e) {
            Services.PluginLog.Error(e, "Exception processing EventFrameWork.ProcessEventPlay");
        }
    }

    public async Task UnloadModules() {
        IsUnloading = true;

        frameworkEventHook?.Disable();

        if (LoadedModules is null) {
            Services.PluginLog.Debug("No modules loaded");
            return;
        }

        Services.PluginLog.Debug("Disposing Module Manager, now disabling all Modules");

        List<Task> tasks = [];

        foreach (var loadedModule in LoadedModules) {
            tasks.Add(Task.Run(async () => {
                if (loadedModule.State is LoadedState.Enabled) {
                    try {
                        Services.PluginLog.Info($"Disabling {loadedModule.Name}");
                        await Task.Run(loadedModule.FeatureBase.Disable);
                        Services.PluginLog.Info($"Successfully Disabled {loadedModule.Name}");
                    }
                    catch (Exception e) {
                        Services.PluginLog.Error(e, $"Error while unloading modification {loadedModule.Name}");
                    }
                }

                await Task.Run(loadedModule.FeatureBase.UnloadAsync);
            }));
        }

        await Task.WhenAll(tasks);

        LoadedModules = null;
    }

    public async Task TryEnableModule(LoadedModule module) {
        if (System.SystemConfig is null) {
            Services.PluginLog.Error("System Config Failed to Load.");
            return;
        }

        if (module.State is LoadedState.Errored) {
            Services.PluginLog.Error($"[{module.Name}] Attempted to enable errored module");
            return;
        }

        try {
            Services.PluginLog.Info($"Enabling {module.Name}");
            await module.FeatureBase.Enable();
            module.State = LoadedState.Enabled;
            Services.PluginLog.Info($"Successfully Enabled {module.Name}");
            System.SystemConfig.EnabledModules.Add(module.Name);
            await System.SystemConfig.Save();
            OnFeatureEnabled?.Invoke();
        }
        catch (Exception e) {
            module.State = LoadedState.Errored;
            module.ErrorMessage = Strings.ModuleManager_LoadFailed;
            Services.PluginLog.Error(e, $"Error while enabling {module.Name}, attempting to disable");

            try {
                await module.FeatureBase.Disable();
                Services.PluginLog.Information($"Successfully disabled erroring module {module.Name}");
            }
            catch (Exception fatal) {
                module.ErrorMessage = Strings.ModuleManager_CriticalError;
                Services.PluginLog.Error(fatal, $"Critical Error while trying to unload erroring module: {module.Name}");
            }
        }
    }

    public async Task TryDisableModification(LoadedModule modification, bool removeFromList = true) {
        if (System.SystemConfig is null) {
            Services.PluginLog.Error("System Config Failed to Load.");
            return;
        }

        if (modification.State is LoadedState.Errored) {
            Services.PluginLog.Error($"[{modification.Name}] Attempted to disable errored modification");
            return;
        }

        try {
            Services.PluginLog.Info($"Disabling {modification.Name}");
            await modification.FeatureBase.Disable();
            modification.FeatureBase.OpenConfigAction = null;
            OnFeatureDisabled?.Invoke();
        }
        catch (Exception e) {
            modification.State = LoadedState.Errored;
            Services.PluginLog.Error(e, $"Failed to Disable {modification.Name}");
        } finally {
            modification.State = LoadedState.Disabled;
            Services.PluginLog.Debug($"Successfully Disabled {modification.Name}");

            if (removeFromList) {
                System.SystemConfig.EnabledModules.Remove(modification.Name);
                await System.SystemConfig.Save();
            }
        }
    }

    public static IEnumerable<LoadedModule> GetModules()
        => System.ModuleManager.LoadedModules?
               .Where(module => module.FeatureBase.ModuleInfo.Type is not (ModuleType.GeneralFeatures or ModuleType.Hidden)) ?? [];

    public ModuleBase? GetModule(string name) {
        if (loadedModulesByName is null) return null;
        if (!loadedModulesByName.TryGetValue(name, out var module)) return null;
        return module.FeatureBase as ModuleBase;
    }

    private static List<FeatureBase> GetAllModules() => Assembly
        .GetCallingAssembly()
        .GetTypes()
        .Where(type => type.IsSubclassOf(typeof(FeatureBase)))
        .Where(type => !type.IsAbstract)
        .Select(type => (FeatureBase?)Activator.CreateInstance(type))
        .Where(modification => modification?.ModuleInfo.Type is not ModuleType.Hidden)
        .OfType<FeatureBase>()
        .ToList();
}
