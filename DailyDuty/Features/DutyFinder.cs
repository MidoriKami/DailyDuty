using System;
using System.Numerics;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Classes;
using KamiToolKit.Controls.Builders;
using KamiToolKit.Services.Controllers;
using KamiToolKit.UI.Nodes;

namespace DailyDuty.Features;

public class DutyFinder : Feature<DutyFinderConfig> {

    public override string DisplayName => Strings.DutyFinder_DisplayName;

    public override DutyFinderConfig Config { get; protected set; } = null!;

    private AddonController<AddonContentsFinder>? addonController;
    private TextNode? timerTextNode;
    private TextButtonNode? openDailyDutyButton;

    public override async Task EnableAsync() {
        Config = await DutyFinderConfig.Load();

        unsafe {
            addonController = new AddonController<AddonContentsFinder> {
                AddonName = "ContentsFinder",
                OnSetup = SetupContentsFinder,
                OnUpdate = UpdateContentsFinder,
                OnFinalize = FinalizeContentsFinder,
            };
        }

        await addonController.EnableAsync();
    }

    public override async Task DisableAsync() {
        await addonController.DisposeAsyncSafe();
        addonController = null;

        timerTextNode = null;
        openDailyDutyButton = null;
    }

    public override void BuildConfigUi(NodeBuilder builder) => builder
        .Spacer(5.0f)
        .Row(Strings.DutyFinder_HideSeconds, rowBuilder => rowBuilder
            .Checkbox("", Config.HideSeconds, val => {
                Config.HideSeconds = val;
                Config.Save();
            }))
        .Row(Strings.DutyFinder_TextColor, rowBuilder => rowBuilder
            .ColorEdit4(Config.Color, ColorHelper.GetColor(8), color => {
                Config.Color = color;
                Config.Save();
            }))
        .Spacer(5.0f)
        .Row(Strings.DutyFinder_ShowOpenButton, rowBuilder => rowBuilder
            .Checkbox("", Config.OpenDailyDutyButton, val => {
                Config.OpenDailyDutyButton = val;
                Config.Save();
            }));

    private unsafe void SetupContentsFinder(AddonContentsFinder* addon) {
        var targetNode = addon->DutyList->CategoryItemRendererList->AtkComponentListItemRenderer->ComponentNode;
        if (targetNode is null) return;

        timerTextNode = new TextNode {
            Position = new Vector2(targetNode->X, targetNode->Y),
            Size = new Vector2(targetNode->Width, targetNode->Height),
            AlignmentType = AlignmentType.Center,
            TextTooltip = Strings.DutyFinder_NextReset,
            String = "0:00:00:00",
            TextColor = Config.Color,
            IsVisible = false,
        };
        timerTextNode.AttachNode(targetNode);

        openDailyDutyButton = new TextButtonNode {
            Position = new Vector2(50.0f, 622.0f),
            Size = new Vector2(130.0f, 28.0f),
            IsVisible = true,
            String = Strings.DutyFinder_OpenDailyDuty,
        };
        openDailyDutyButton.AddEvent(AtkEventType.ButtonClick, () => System.ConfigWindow.Toggle());
        openDailyDutyButton.AttachNode(addon->RootNode);
    }

    private unsafe void UpdateContentsFinder(AddonContentsFinder* addon) {
        var nextReset = Time.NextDailyReset();
        var timeRemaining = nextReset - DateTime.UtcNow;

        timerTextNode?.String = timeRemaining.FormatTime(Config.HideSeconds);
        timerTextNode?.TextColor = Config.Color;

        if (timerTextNode?.IsVisible != addon->SelectedRadioButton is 0) {
            timerTextNode?.IsVisible = addon->SelectedRadioButton is 0;
            addon->UpdateCollisionNodeList(false);
        }

        openDailyDutyButton?.IsVisible = Config.OpenDailyDutyButton;
    }

    private unsafe void FinalizeContentsFinder(AddonContentsFinder* _) {
        timerTextNode?.Dispose();
        timerTextNode = null;

        openDailyDutyButton?.Dispose();
        openDailyDutyButton = null;
    }
}

public class DutyFinderConfig : FeatureConfigFile<DutyFinderConfig> {
    protected override string FileName => "DutyFinderEnhancements.config.json";

    public Vector4 Color { get; set; } = ColorHelper.GetColor(8);
    public bool HideSeconds { get; set; }
    public bool OpenDailyDutyButton { get; set; } = true;
}
