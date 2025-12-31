using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Extensions;

public static class AddonLifecycleExtensions {
    extension(IAddonLifecycle addonLifecycle) {
        public void LogAddon(string addonName, params AddonEvent[] loggedModules) {
            if (loggedModules.Length is 0) {
                loggedModules = [
                    AddonEvent.PostSetup, AddonEvent.PostOpen,
                    AddonEvent.PostClose, AddonEvent.PostShow, 
                    AddonEvent.PostHide, AddonEvent.PostRefresh,
                    AddonEvent.PostRequestedUpdate, AddonEvent.PreFinalize,
                ];
            }

            ActiveLoggers.TryAdd(addonName, loggedModules.ToList());
            foreach (var loggedModule in loggedModules) {
                addonLifecycle.RegisterListener(loggedModule, addonName, Logger);
            }
        }

        public void UnLogAddon(string addonName) {
            if (!ActiveLoggers.TryGetValue(addonName, out var loggedModules)) return;

            foreach (var loggedModule in loggedModules) {
                addonLifecycle.UnregisterListener(loggedModule, addonName, Logger);
            }
        }
    }

    private static readonly Dictionary<string, List<AddonEvent>> ActiveLoggers = [];

    private static void Logger(AddonEvent type, AddonArgs args) {
        switch (args) {
            case AddonReceiveEventArgs receiveEventArgs:
                Services.PluginLog.Debug($"[{args.AddonName}] {(AtkEventType)receiveEventArgs.AtkEventType}: {receiveEventArgs.EventParam}");
                break;
            
            default:
                Services.PluginLog.Debug($"{args.AddonName} called {type.ToString().Replace("Post", string.Empty)}");
                break;
        }
    }
}
