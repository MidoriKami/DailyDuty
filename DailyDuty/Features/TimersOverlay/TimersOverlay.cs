using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Windows;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Overlay;
using KamiToolKit.Premade.Addons;

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
    public ColorPickerAddon? ColorPicker;
    
    public Config ModuleConfig = null!;
    public override NodeBase DisplayNode => new ConfigNode(this);

    protected override void OnFeatureLoad() {
        ModuleConfig = Utilities.Config.LoadCharacterConfig<Config>($"{ModuleInfo.FileName}.config.json");
        if (ModuleConfig is null) throw new Exception("Failed to load config file");
        
        ModuleConfig.FileName = ModuleInfo.FileName;
        System.ModuleManager.OnLoadComplete += RebuildTimers;
    }

    protected override void OnFeatureUnload() {
        ModuleConfig = null!;
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
                SelectedOptions = ModuleConfig.EnabledTimers,
                InternalName = "TimersSelection",
                Title = "Timer Selection",
                OnEdited = () => {
                    foreach (var (index, option) in ModuleConfig.EnabledTimers.Index()) {
                        if (!ModuleConfig.TimerData.ContainsKey(option)) {
                            var position = new Vector2(AtkStage.Instance()->ScreenSize.Width / 2.0f, AtkStage.Instance()->ScreenSize.Height / 3.0f);
                            var offset = new Vector2(0.0f, 68.0f) * index;
                        
                            ModuleConfig.TimerData.TryAdd(option, new TimerData {
                                Position = position + offset,
                            });
                        }
                    }

                    ModuleConfig.MarkDirty();
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
        
        ColorPicker?.Dispose();
        ColorPicker = null;
    }
    
    protected override void OnFeatureUpdate() {
        if (ModuleConfig.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            ModuleConfig.Save();
        }
    }
    
    private void RebuildTimers() {
        if (!System.ModuleManager.IsLoadComplete) return;
        System.ModuleManager.OnLoadComplete -= RebuildTimers;

        overlayController?.RemoveAllNodes();
        if (!IsEnabled) return;

        foreach (var (index, option) in ModuleConfig.EnabledTimers.Index()) {
            var loadedModule = System.ModuleManager.LoadedModules?.FirstOrDefault(loadedModule => loadedModule.Name == option);
            if (loadedModule?.FeatureBase is not ModuleBase module) continue;
            
            Vector2 initialPosition;

            if (ModuleConfig.TimerData.TryGetValue(option, out var data)) {
                initialPosition = data.Position;
            }
            else {
                var position = new Vector2(AtkStage.Instance()->ScreenSize.Width / 2.0f, AtkStage.Instance()->ScreenSize.Height / 3.0f);
                var offset = new Vector2(0.0f, 68.0f) * index;
                
                initialPosition = position + offset;
            }
            
            overlayController?.CreateNode(() => new TimerNode {
                Size = new Vector2(300.0f, 64.0f),
                Position = initialPosition,
                TimerConfig = ModuleConfig,
                Module = module,
            });
        }
    }
}
