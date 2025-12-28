using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using DailyDuty.Windows;
using Dalamud.Plugin.Services;
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
    
    public TimersOverlayConfig ModuleConfig = null!;
    public override NodeBase DisplayNode => new ConfigNode(this);

    private bool isEnabled;
    
    public override void Load() {
        ModuleConfig = Config.LoadCharacterConfig<TimersOverlayConfig>($"{ModuleInfo.FileName}.config.json");
        if (ModuleConfig is null) throw new Exception("Failed to load config file");
        
        ModuleConfig.FileName = ModuleInfo.FileName;

        Services.Framework.Update += OnFrameworkUpdate;
    }

    public override void Unload() {
        Services.Framework.Update -= OnFrameworkUpdate;
        
        ModuleConfig = null!;
    }

    public override void Enable() {
        isEnabled = true;

        overlayController = new OverlayController();

        OpenConfigAction = () => {
            if (System.ModuleManager.LoadedModules is null) return;
            
            moduleSelectionWindow ??= new MultiSelectWindow {
                Size = new Vector2(225.0f, 300.0f),
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

                    ModuleConfig.SavePending = true;
                    RebuildTimers();
                },
            };
            
            moduleSelectionWindow.Toggle();
        };
        
        RebuildTimers();
    }

    public override void Disable() {
        isEnabled = false;
        
        moduleSelectionWindow?.Dispose();
        moduleSelectionWindow = null;
        
        overlayController?.Dispose();
        overlayController = null;
        
        ColorPicker?.Dispose();
        ColorPicker = null;
    }
    
    private void OnFrameworkUpdate(IFramework framework) {
        if (ModuleConfig.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            ModuleConfig.Save();
        }
    }
    
    private void RebuildTimers() {
        overlayController?.RemoveAllNodes();
        if (!isEnabled) return;

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
