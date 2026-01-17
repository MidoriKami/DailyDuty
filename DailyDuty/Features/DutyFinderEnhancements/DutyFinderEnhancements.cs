using System;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Controllers;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.DutyFinderEnhancements;

public unsafe class DutyFinderEnhancements : FeatureBase {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Duty Finder Enhancements",
        FileName = "DutyFinderEnhancements",
        Type = ModuleType.GeneralFeatures,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Countdown", "Reset" ],
    };

    public DutyFinderEnhancementsConfig ModuleDutyFinderEnhancementsConfig = null!;
    private AddonController<AddonContentsFinder>? addonController;
    private TextNode? timerTextNode;
    private TextButtonNode? openDailyDutyButton;
    
    public override NodeBase DisplayNode => new DutyFinderEnhancementsConfigNode(this);
    
    protected override void OnFeatureLoad() {
        ModuleDutyFinderEnhancementsConfig = Config.LoadCharacterConfig<DutyFinderEnhancementsConfig>($"{ModuleInfo.FileName}.config.json");
        if (ModuleDutyFinderEnhancementsConfig is null) throw new Exception("Failed to load config file");
        
        ModuleDutyFinderEnhancementsConfig.FileName = ModuleInfo.FileName;
    }

    protected override void OnFeatureUnload() {
        ModuleDutyFinderEnhancementsConfig = null!;
    }

    protected override void OnFeatureEnable() {
        addonController = new AddonController<AddonContentsFinder>("ContentsFinder");

        addonController.OnAttach += addon => {
            var targetNode = addon->DutyList->CategoryItemRendererList->AtkComponentListItemRenderer->ComponentNode;
            if (targetNode is null) return;
            
            timerTextNode = new TextNode {
                Position = new Vector2(targetNode->X, targetNode->Y),
                Size = new Vector2(targetNode->Width, targetNode->Height),
                AlignmentType = AlignmentType.Center,
                TextTooltip = "[DailyDuty] Time until next daily reset",
                String = "0:00:00:00",
                TextColor = ModuleDutyFinderEnhancementsConfig.Color,
                IsVisible = false,
            };
            timerTextNode.AttachNode(targetNode);
            
            openDailyDutyButton = new TextButtonNode {
                Position = new Vector2(50.0f, 622.0f),
                Size = new Vector2(130.0f, 28.0f),
                IsVisible = true,
                String = "Open DailyDuty",
            };
            openDailyDutyButton.AddEvent(AtkEventType.ButtonClick, () => System.ConfigurationWindow.Toggle());
            openDailyDutyButton.AttachNode(addon->RootNode);
        };

        addonController.OnDetach += _ => {
            timerTextNode?.Dispose();
            timerTextNode = null;
            
            openDailyDutyButton?.Dispose();
            openDailyDutyButton = null;
        };
        
        addonController.OnUpdate += addon => {
            var nextReset = Time.NextDailyReset();
            var timeRemaining = nextReset - DateTime.UtcNow;

            timerTextNode?.String = timeRemaining.FormatTimeSpanShort(ModuleDutyFinderEnhancementsConfig.HideSeconds);
            timerTextNode?.TextColor = ModuleDutyFinderEnhancementsConfig.Color;

            if (timerTextNode?.IsVisible != addon->SelectedRadioButton is 0) {
                timerTextNode?.IsVisible = addon->SelectedRadioButton is 0;
                addon->UpdateCollisionNodeList(false);
            }

            openDailyDutyButton?.IsVisible = ModuleDutyFinderEnhancementsConfig.OpenDailyDutyButton;
        };
        
        addonController.Enable();
    }

    protected override void OnFeatureDisable() {
        addonController?.Dispose();
        addonController = null;

        timerTextNode?.Dispose();
        timerTextNode = null;
    }

    protected override void OnFeatureUpdate() {
        if (ModuleDutyFinderEnhancementsConfig.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            ModuleDutyFinderEnhancementsConfig.Save();
        }
    }
}
