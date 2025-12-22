using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DailyDuty.Enums;

namespace DailyDuty.Classes;

public class ModuleManager : IDisposable {

    public List<LoadedModule>? LoadedModules { get; private set; }
    
    public void Dispose() => UnloadModules();
    
    public void LoadModules() {
        var allModules = GetModules();
        LoadedModules = [];
        
        foreach (var module in allModules) {
            Services.PluginInterface.Inject(module);

            var newLoadedModule = new LoadedModule(module, LoadedState.Disabled);

            LoadedModules.Add(newLoadedModule);

            if (System.SystemConfig?.EnabledModules.Contains(module.Name) ?? false) {
                TryEnableModule(newLoadedModule);
            }
        }
    }

    public void UnloadModules() {
        if (LoadedModules is null) {
            Services.PluginLog.Error("No loaded modules loaded");
            return;
        }
        
        Services.PluginLog.Debug("Disposing Module Manager, now disabling all Modules");
        
        foreach (var loadedModule in LoadedModules) {
            if (loadedModule.State is LoadedState.Enabled) {
                try {
                    Services.PluginLog.Info($"Disabling {loadedModule.Name}");
                    loadedModule.ModuleBase.Disable();
                    Services.PluginLog.Info($"Successfully Disabled {loadedModule.Name}");
                }
                catch (Exception e) {
                    Services.PluginLog.Error(e, $"Error while unloading modification {loadedModule.Name}");
                }
            }
        }

        LoadedModules = null;
    }
    
    public static void TryEnableModule(LoadedModule module) {
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
            module.ModuleBase.Enable();
            module.State = LoadedState.Enabled;
            Services.PluginLog.Info($"Successfully Enabled {module.Name}");
            System.SystemConfig.EnabledModules.Add(module.Name);
            System.SystemConfig.Save();
        }
        catch (Exception e) {
            module.State = LoadedState.Errored;
            module.ErrorMessage = "Failed to load, this module has been disabled.";
            Services.PluginLog.Error(e, $"Error while enabling {module.Name}, attempting to disable");
            
            try {
                module.ModuleBase.Disable();
                Services.PluginLog.Information($"Successfully disabled erroring module {module.Name}");
            }
            catch (Exception fatal) {
                module.ErrorMessage = "Critical Error: Module failed to load, and errored again while unloading.";
                Services.PluginLog.Error(fatal, $"Critical Error while trying to unload erroring module: {module.Name}");
            }
        }
    }

    public static void TryDisableModification(LoadedModule modification, bool removeFromList = true) {
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
            modification.ModuleBase.Disable();
            modification.ModuleBase.OpenConfigAction = null;
        }
        catch (Exception e) {
            modification.State = LoadedState.Errored;
            Services.PluginLog.Error(e, $"Failed to Disable {modification.Name}");
        } finally {
            modification.State = LoadedState.Disabled;
            Services.PluginLog.Debug($"Successfully Disabled {modification.Name}");

            if (removeFromList) {
                System.SystemConfig.EnabledModules.Remove(modification.Name);
                System.SystemConfig.Save();
            }
        }
    }
    
    private static List<ModuleBase> GetModules() => Assembly
        .GetCallingAssembly()
        .GetTypes()
        .Where(type => type.IsSubclassOf(typeof(ModuleBase)))
        .Where(type => !type.IsAbstract)
        .Select(type => (ModuleBase?) Activator.CreateInstance(type))
        .Where(modification => modification?.ModuleInfo.Type is not ModuleType.Hidden)
        .OfType<ModuleBase>()
        .ToList();

}
