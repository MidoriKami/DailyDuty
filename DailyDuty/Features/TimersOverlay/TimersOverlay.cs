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
using KamiToolKit.BaseTypes;
using KamiToolKit.UiOverlay;

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

    private Dictionary<LoadedModule, TimerOverlayNode?>? timerNodes;

    protected override async Task OnFeatureLoad() {
        ModuleTimersOverlayConfig = await Config.LoadCharacterConfig<TimersOverlayConfig>($"{ModuleInfo.FileName}.config.json");
        if (ModuleTimersOverlayConfig is null) throw new Exception("Failed to load config file");

        ModuleTimersOverlayConfig.FileName = ModuleInfo.FileName;
    }

    protected override Task OnFeatureUnload() {
        ModuleTimersOverlayConfig = null!;

        return Task.CompletedTask;
    }

    protected override async Task OnFeatureEnable() {
        timerNodes = [];

        OpenConfigAction = OpenTimerConfigWindow;

        foreach (var module in System.ModuleManager.LoadedModules ?? []) {
            if (module.FeatureBase is not ModuleBase) continue;

            timerNodes.Add(module, null);
        }

        await Services.Framework.RunSafely(() => {
            overlayController = new OverlayController();
        });

        System.ModuleManager.OnFeatureEnabled += RebuildTimers;
        System.ModuleManager.OnFeatureDisabled += RebuildTimers;
    }

    protected override async Task OnFeatureDisable() {
        System.ModuleManager.OnFeatureEnabled -= RebuildTimers;
        System.ModuleManager.OnFeatureDisabled -= RebuildTimers;

        await Services.Framework.RunSafely(() => overlayController?.Dispose());
        overlayController = null;
        timerNodes?.Clear();
        timerNodes = null;

        await Task.WhenAll(moduleSelectionWindow?.DisposeAsync().AsTask() ?? Task.CompletedTask);
        moduleSelectionWindow = null;
    }

    protected override void OnFeatureUpdate() {
        if (ModuleTimersOverlayConfig.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            ModuleTimersOverlayConfig.Save();
        }
    }

    private unsafe void OpenTimerConfigWindow() {
        if (System.ModuleManager.LoadedModules is null) return;

        moduleSelectionWindow ??= new MultiSelectWindow {
            Size = new Vector2(300.0f, 300.0f),
            Options = System.ModuleManager.LoadedModules.Where(loadedModule => loadedModule.FeatureBase.ModuleInfo.Type is not ModuleType.GeneralFeatures)
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
    }

    private void RebuildTimers()
        => Task.Run(RebuildTimersTask);

    private async Task RebuildTimersTask() {
        if (!IsEnabled) {
            await Services.Framework.RunSafely(() => overlayController?.RemoveAllNodes());

            foreach (var (key, _) in timerNodes ?? []) {
                timerNodes?[key] = null;
            }

            return;
        }

        if (timerNodes is null) return;
        if (overlayController is null) return;

        await Services.Framework.RunSafely(() => {
            Vector2 screenSize;
            unsafe {
                screenSize = AtkStage.Instance()->ScreenSize;
            }

            foreach (var (index, (loadedModule, timerEntry)) in timerNodes.Index()) {
                if (loadedModule.FeatureBase is not ModuleBase module) continue;
                var moduleName = loadedModule.FeatureBase.ModuleInfo.DisplayName;

                var shouldHaveTimer = ModuleTimersOverlayConfig.EnabledTimers.Contains(moduleName);

                if (timerEntry is null && shouldHaveTimer) {
                    Vector2 nodePosition;

                    var col = index % 5;
                    var row = index / 5;

                    if (ModuleTimersOverlayConfig.TimerData.TryGetValue(moduleName, out var data) && data.Position.Y < screenSize.Y) {
                        nodePosition = data.Position;
                    }
                    else {
                        nodePosition =
                            new Vector2(screenSize.X / 3.0f, screenSize.Y / 6.0f) +
                            new Vector2(row * 300.0f + 12.0f, col * 64.0f + 12.0f);
                    }

                    var newTimerNode = new TimerOverlayNode {
                        Size = new Vector2(300.0f, 64.0f),
                        Position = nodePosition,
                        TimerTimersOverlayConfig = ModuleTimersOverlayConfig,
                        Module = module,
                    };

                    overlayController.AddNode(newTimerNode);
                    timerNodes[loadedModule] = newTimerNode;
                }
                else if (timerEntry is not null && !shouldHaveTimer) {
                    overlayController.RemoveNode(timerEntry);
                    timerNodes[loadedModule] = null;
                }
            }
        });
    }
}
