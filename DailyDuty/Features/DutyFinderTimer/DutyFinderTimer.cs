using System;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Classes.Controllers;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Addons;

namespace DailyDuty.Features.DutyFinderTimer;

public unsafe class DutyFinderTimer : FeatureBase {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Duty Finder Timer",
        FileName = "DutyFinderTimer",
        Type = ModuleType.GeneralFeatures,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Countdown", "Reset" ],
    };

    public Config ModuleConfig = null!;
    public ColorPickerAddon? ColorPicker;
    private AddonController<AddonContentsFinder>? addonController;
    private TextNode? timerTextNode;
    
    public override NodeBase DisplayNode => new ConfigNode(this);
    
    protected override void OnFeatureLoad() {
        ModuleConfig = Utilities.Config.LoadCharacterConfig<Config>($"{ModuleInfo.FileName}.config.json");
        if (ModuleConfig is null) throw new Exception("Failed to load config file");
        
        ModuleConfig.FileName = ModuleInfo.FileName;
    }

    protected override void OnFeatureUnload() {
        ModuleConfig = null!;
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
                TextColor = ModuleConfig.Color,
            };
            timerTextNode.AddFlags(NodeFlags.HasCollision);
            timerTextNode.AttachNode(targetNode);
        };

        addonController.OnDetach += _ => {
            timerTextNode?.Dispose();
            timerTextNode = null;
        };
        
        addonController.OnUpdate += _ => {
            var nextReset = Time.NextDailyReset();
            var timeRemaining = nextReset - DateTime.UtcNow;

            timerTextNode?.String = timeRemaining.FormatTimeSpanShort(ModuleConfig.HideSeconds);
            timerTextNode?.TextColor = ModuleConfig.Color;
        };
        
        addonController.Enable();
    }

    protected override void OnFeatureDisable() {
        addonController?.Dispose();
        addonController = null;
        
        ColorPicker?.Dispose();
        ColorPicker = null;

        timerTextNode?.Dispose();
        timerTextNode = null;
    }

    protected override void OnFeatureUpdate() {
        if (ModuleConfig.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            ModuleConfig.Save();
        }
    }
}
