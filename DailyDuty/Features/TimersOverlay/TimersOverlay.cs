using DailyDuty.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Windows;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Overlay.UiOverlay;

namespace DailyDuty.Features.TimersOverlay;

public class TimersOverlay : FeatureBase {

    public override ModuleInfo ModuleInfo => new() {
        DisplayName = Strings.TimersOverlay_DisplayName,
        FileName = "Timers",
        Type = ModuleType.GeneralFeatures,
        Tags = ["Countdown", "Reset"],
    };

    private MultiSelectWindow? moduleSelectionWindow;
    private OverlayController? overlayController;

    public TimersOverlayConfig ModuleTimersOverlayConfig = null!;
    public override NodeBase DisplayNode => new TimersOverlayConfigNode(this);

    protected override async Task OnFeatureLoad() {
        ModuleTimersOverlayConfig = await Config.LoadCharacterConfig<TimersOverlayConfig>($"{ModuleInfo.FileName}.config.json");
        if (ModuleTimersOverlayConfig is null) throw new Exception("Failed to load config file");

        ModuleTimersOverlayConfig.FileName = ModuleInfo.FileName;

        System.ModuleManager.OnLoadComplete += RebuildTimers;
    }

    protected override Task OnFeatureUnload() {
        ModuleTimersOverlayConfig = null!;

        return Task.CompletedTask;
    }

    protected override async Task OnFeatureEnable() {
        unsafe {
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
                    Title = Strings.TimersOverlay_Selection,
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
        }

        await Services.Framework.Run(() => {
            overlayController = new OverlayController();
            RebuildTimers();
        });

        System.ModuleManager.OnFeatureEnabled += RebuildTimers;
        System.ModuleManager.OnFeatureDisabled += RebuildTimers;
    }

    protected override async Task OnFeatureDisable() {
        System.ModuleManager.OnFeatureEnabled -= RebuildTimers;
        System.ModuleManager.OnFeatureDisabled -= RebuildTimers;

        await Services.Framework.Run(() => overlayController?.Dispose());

        await Task.WhenAll(moduleSelectionWindow?.DisposeAsync().AsTask() ?? Task.CompletedTask);
        moduleSelectionWindow = null;
    }

    protected override void OnFeatureUpdate() {
        if (ModuleTimersOverlayConfig.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            ModuleTimersOverlayConfig.Save();
        }
    }

    private void RebuildTimers()
        => Task.Run(RebuildTimersTask);

    private async Task RebuildTimersTask() {
        if (!System.ModuleManager.IsLoadComplete) return;
        System.ModuleManager.OnLoadComplete -= RebuildTimers;

        await Services.Framework.Run(() => overlayController?.RemoveAllNodes());
        if (!IsEnabled) return;

        var nodesToCreate = new List<(Vector2 Position, ModuleBase Module)>();

        foreach (var (index, option) in ModuleTimersOverlayConfig.EnabledTimers.Index()) {
            var loadedModule = System.ModuleManager.LoadedModules?.FirstOrDefault(loadedModule => loadedModule.Name == option);
            if (loadedModule?.FeatureBase is not ModuleBase module) continue;
            if (!module.IsEnabled) continue;

            Vector2 initialPosition;

            unsafe {
                if (ModuleTimersOverlayConfig.TimerData.TryGetValue(option, out var data)) {
                    initialPosition = data.Position;
                }
                else {
                    var position = new Vector2(AtkStage.Instance()->ScreenSize.Width / 2.0f, AtkStage.Instance()->ScreenSize.Height / 3.0f);
                    var offset = new Vector2(0.0f, 68.0f) * index;

                    initialPosition = position + offset;
                }
            }

            nodesToCreate.Add((initialPosition, module));
        }

        await Services.Framework.Run(() => {
            foreach (var node in nodesToCreate) {
                overlayController?.AddNode(new TimerOverlayNode {
                    Size = new Vector2(300.0f, 64.0f),
                    Position = node.Position,
                    TimerTimersOverlayConfig = ModuleTimersOverlayConfig,
                    Module = node.Module,
                });
            }
        });
    }
}
