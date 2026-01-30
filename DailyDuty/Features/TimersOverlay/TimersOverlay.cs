using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using DailyDuty.Windows;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Overlay;

namespace DailyDuty.Features.TimersOverlay;

public unsafe class TimersOverlay : FeatureBase {

    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Timers Overlay",
        FileName = "Timers",
        Type = ModuleType.GeneralFeatures,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Countdown", "Reset" ],
    };

    private MultiSelectWindow? moduleSelectionWindow;
    private OverlayController? overlayController;
    
    public TimersOverlayConfig ModuleTimersOverlayConfig = null!;
    public override NodeBase DisplayNode => new TimersOverlayConfigNode(this);

    protected override void OnFeatureLoad() {
        ModuleTimersOverlayConfig = Config.LoadCharacterConfig<TimersOverlayConfig>($"{ModuleInfo.FileName}.config.json");
        if (ModuleTimersOverlayConfig is null) throw new Exception("Failed to load config file");
        
        ModuleTimersOverlayConfig.FileName = ModuleInfo.FileName;
        System.ModuleManager.OnLoadComplete += RebuildTimers;
    }

    protected override void OnFeatureUnload() {
        ModuleTimersOverlayConfig = null!;
    }

    protected override void OnFeatureEnable() {
        overlayController = new OverlayController();

        OpenConfigAction = () => {
            if (System.ModuleManager.LoadedModules is null) return;
            
            moduleSelectionWindow ??= new MultiSelectWindow {
                Size = new Vector2(300.0f, 300.0f),
                Options = System.ModuleManager.LoadedModules
                    .Where(loadedModule => loadedModule.FeatureBase.ModuleInfo.Type is not ModuleType.GeneralFeatures)
                    .Select(loadedModule => loadedModule.Name)
                    .ToList(),
                SelectedOptions = ModuleTimersOverlayConfig.EnabledTimers,
                InternalName = "TimersSelection",
                Title = "Timer Selection",
                OnEdited = () => {
                    foreach (var (index, option) in ModuleTimersOverlayConfig.EnabledTimers.Index()) {
                        if (!ModuleTimersOverlayConfig.TimerData.ContainsKey(option)) {
                            var position = new Vector2(AtkStage.Instance()->ScreenSize.Width / 2.0f, AtkStage.Instance()->ScreenSize.Height / 3.0f);
                            var offset = new Vector2(0.0f, 68.0f) * index;
                        
                            ModuleTimersOverlayConfig.TimerData.TryAdd(option, new TimersOverlayTimerData {
                                Position = position + offset,
                            });
                        }
                    }

                    ModuleTimersOverlayConfig.MarkDirty();
                    RebuildTimers();
                },
            };
            
            moduleSelectionWindow.Toggle();
        };
        
        RebuildTimers();
    }

    protected override void OnFeatureDisable() {
        moduleSelectionWindow?.Dispose();
        moduleSelectionWindow = null;
        
        overlayController?.Dispose();
        overlayController = null;
    }
    
    protected override void OnFeatureUpdate() {
        if (ModuleTimersOverlayConfig.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            ModuleTimersOverlayConfig.Save();
        }
    }
    
    private void RebuildTimers() {
        if (!System.ModuleManager.IsLoadComplete) return;
        System.ModuleManager.OnLoadComplete -= RebuildTimers;

        overlayController?.RemoveAllNodes();
        if (!IsEnabled) return;

        foreach (var (index, option) in ModuleTimersOverlayConfig.EnabledTimers.Index()) {
            var loadedModule = System.ModuleManager.LoadedModules?.FirstOrDefault(loadedModule => loadedModule.Name == option);
            if (loadedModule?.FeatureBase is not ModuleBase module) continue;
            
            Vector2 initialPosition;

            if (ModuleTimersOverlayConfig.TimerData.TryGetValue(option, out var data)) {
                initialPosition = data.Position;
            }
            else {
                var position = new Vector2(AtkStage.Instance()->ScreenSize.Width / 2.0f, AtkStage.Instance()->ScreenSize.Height / 3.0f);
                var offset = new Vector2(0.0f, 68.0f) * index;
                
                initialPosition = position + offset;
            }
            
            overlayController?.CreateNode(() => new TimerOverlayNode {
                Size = new Vector2(300.0f, 64.0f),
                Position = initialPosition,
                TimerTimersOverlayConfig = ModuleTimersOverlayConfig,
                Module = module,
            });
        }
    }
}
