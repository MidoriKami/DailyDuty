using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using DailyDuty.Windows;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Classes;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using InstanceContent = FFXIVClientStructs.FFXIV.Client.Game.UI.InstanceContent;
using SeStringBuilder = Lumina.Text.SeStringBuilder;

namespace DailyDuty.Modules;

public class DutyRouletteData : ModuleTaskData<ContentRoulette> {
    public int ExpertTomestones;
    public int ExpertTomestoneCap;
    public bool AtTomeCap;

    protected override void DrawModuleData() {
        DrawDataTable(
            (Strings.CurrentWeeklyTomestones, ExpertTomestones.ToString()),
            (Strings.WeeklyTomestoneLimit, ExpertTomestoneCap.ToString()),
            (Strings.AtWeeklyTomestoneLimit, AtTomeCap.ToString())
        );
        
        base.DrawModuleData();
    }
}

public class DutyRouletteConfig : ModuleTaskConfig<ContentRoulette> {
    public bool CompleteWhenCapped;
    public bool ClickableLink = true;
    public bool ColorContentFinder = true;
    public Vector4 CompleteColor = KnownColor.LimeGreen.Vector();
    public Vector4 IncompleteColor = KnownColor.OrangeRed.Vector();
    public bool ShowOpenDailyDutyButton = true;
    public bool ShowResetTimer = true;
    public Vector4 TimerColor = KnownColor.Black.Vector();
    
    protected override void DrawModuleConfig() {
        ConfigChanged |= ImGui.Checkbox(Strings.ClickableLink, ref ClickableLink);
        ConfigChanged |= ImGui.Checkbox(Strings.CompleteWhenTomeCapped, ref CompleteWhenCapped);
        ConfigChanged |= ImGui.Checkbox("Show 'Open DailyDuty' button", ref ShowOpenDailyDutyButton);
        
        ImGui.Spacing();

        ConfigChanged |= ImGui.Checkbox("Show Daily Reset Timer in Duty Finder", ref ShowResetTimer);

        if (ShowResetTimer) {
            ConfigChanged |= ImGuiTweaks.ColorEditWithDefault("Timer Color", ref TimerColor, ColorHelper.GetColor(7));
        }
        
        ImGui.Spacing();

        ConfigChanged |= ImGui.Checkbox("Color Duty Finder", ref ColorContentFinder);
        
        if (ColorContentFinder) {
            ImGuiHelpers.ScaledDummy(5.0f);

            ConfigChanged |= ImGuiTweaks.ColorEditWithDefault("Complete Color", ref CompleteColor, KnownColor.LimeGreen.Vector());
            ConfigChanged |= ImGuiTweaks.ColorEditWithDefault("Incomplete Color", ref IncompleteColor, KnownColor.OrangeRed.Vector());
        }
        
        ImGuiHelpers.ScaledDummy(5.0f);
        
        base.DrawModuleConfig();
    }
}

public unsafe class DutyRoulette : BaseModules.Modules.DailyTask<DutyRouletteData, DutyRouletteConfig, ContentRoulette> {
    public override ModuleName ModuleName => ModuleName.DutyRoulette;

    public override bool HasClickableLink => Config.ClickableLink;
    
    public override PayloadId ClickableLinkPayloadId => PayloadId.OpenDutyFinderRoulette;

    public override bool HasTooltip => true;

    private TextNode? infoTextNode;
    private TextButtonNode? openDailyDutyButton;
    private TextNode? dailyResetTimer;

    private readonly ContentFinderRouletteListController rouletteListController;

    public DutyRoulette() {
        rouletteListController = new ContentFinderRouletteListController();
        rouletteListController.Apply += ApplyListModification;
        rouletteListController.Update += UpdateListModification;
        rouletteListController.Reset += ResetListModification;
        
        System.ContentsFinderController.OnAttach += AttachNodes;
        System.ContentsFinderController.OnDetach += DetachNodes;
        System.ContentsFinderController.OnUpdate += OnContentFinderUpdate;
    }

    public override void Dispose() {
        System.ContentsFinderController.OnAttach -= AttachNodes;
        System.ContentsFinderController.OnDetach -= DetachNodes;
        System.ContentsFinderController.OnUpdate -= OnContentFinderUpdate;
        
        rouletteListController.Dispose();
    }

    private void ApplyListModification(ListPopulatorData<AddonContentsFinder> obj) {
        if (Config.ColorContentFinder) {
            var roulette = GetRoulette(obj);
            var dutyNameTextNode = (AtkTextNode*) obj.NodeList[3];
            
            // If this roulette is being tracked, apply color
            if (Config.TaskConfig.FirstOrDefault(task => task.RowId == roulette.RowId) is { Enabled: true }) {
                var isRouletteCompleted = InstanceContent.Instance()->IsRouletteComplete((byte) roulette.RowId);
                dutyNameTextNode->TextColor = isRouletteCompleted ? Config.CompleteColor.ToByteColor() : Config.IncompleteColor.ToByteColor();
            }
        }
    }
    
    private void UpdateListModification(ListPopulatorData<AddonContentsFinder> obj) {
        var dutyNameTextNode = (AtkTextNode*) obj.NodeList[3];
        var levelTextNode = (AtkTextNode*) obj.NodeList[4];
        
        if (Config.ColorContentFinder) {
            var roulette = GetRoulette(obj);
            
            // If this roulette is being tracked, apply color
            if (Config.TaskConfig.FirstOrDefault(task => task.RowId == roulette.RowId) is { Enabled: true }) {
                var isRouletteCompleted = InstanceContent.Instance()->IsRouletteComplete((byte) roulette.RowId);
                dutyNameTextNode->TextColor = isRouletteCompleted ? Config.CompleteColor.ToByteColor() : Config.IncompleteColor.ToByteColor();
            }
            else {
                dutyNameTextNode->TextColor = levelTextNode->TextColor;
                rouletteListController.UntrackElement(obj.Index);
            }
        }
        else {
            dutyNameTextNode->TextColor = levelTextNode->TextColor;
            rouletteListController.UntrackElement(obj.Index);
        }
    }

    private static void ResetListModification(ListPopulatorData<AddonContentsFinder> obj) {
        var dutyNameTextNode = (AtkTextNode*) obj.NodeList[3];
        var levelTextNode = (AtkTextNode*) obj.NodeList[4];
        
        dutyNameTextNode->TextColor = levelTextNode->TextColor;
    }

    private static ContentRoulette GetRoulette(ListPopulatorData<AddonContentsFinder> obj) {
        var contentId = obj.ItemInfo->ListItem->UIntValues[1];
        var contentEntry = AgentContentsFinder.Instance()->ContentList[contentId - 1];
        var contentData = contentEntry.Value->Id;
        return Service.DataManager.GetExcelSheet<ContentRoulette>().GetRow(contentData.Id);
    }

    private void AttachNodes(AddonContentsFinder* addon) {
        var targetResNode = addon->GetNodeById(56);
        if (targetResNode is null) return;

        infoTextNode = new TextNode {
            NodeId = 1000,
            X = 16.0f,
            Y = targetResNode->GetYFloat() + 2.0f, 
            TextFlags = TextFlags.AutoAdjustNodeSize,
            AlignmentType = AlignmentType.TopLeft,
            Text = GetHintText(),
            Tooltip = "Feature from DailyDuty Plugin",
            EnableEventFlags = true,
            IsVisible = false,
        };
        System.NativeController.AttachNode(infoTextNode, targetResNode, NodePosition.AfterTarget);
        
        openDailyDutyButton = new TextButtonNode {
            Position = new Vector2(50.0f, 622.0f),
            Size = new Vector2(130.0f, 28.0f),
            IsVisible = true,
            Label = "Open DailyDuty",
        };
        openDailyDutyButton.AddEvent(AddonEventType.ButtonClick, _ => System.WindowManager.GetWindow<ConfigurationWindow>()?.UnCollapseOrToggle() );
        System.NativeController.AttachNode(openDailyDutyButton, addon->RootNode);

        var targetComponent = GetListHeaderComponentNode(addon);
        if (targetComponent is not null) {
            dailyResetTimer = new TextNode {
                Position = new Vector2(targetComponent->X, targetComponent->Y),
                Size = new Vector2(targetComponent->Width, targetComponent->Height),
                AlignmentType = AlignmentType.Center,
                Tooltip = "[DailyDuty] Time until next daily reset",
                Text = "0:00:00:00",
                EnableEventFlags = true,
                TextColor = Config.TimerColor,
            };
            System.NativeController.AttachNode(dailyResetTimer, targetComponent);
        }
        
        if (Config.TimerColor == Vector4.Zero) {
            Config.TimerColor = ColorHelper.GetColor(7);
            ConfigChanged = true;
        }
    }

    private void DetachNodes(AddonContentsFinder* addon) {
        System.NativeController.DisposeNode(ref infoTextNode);
        System.NativeController.DisposeNode(ref openDailyDutyButton);
        System.NativeController.DisposeNode(ref dailyResetTimer);
    }

    private void OnContentFinderUpdate(AddonContentsFinder* addon) {
        if (openDailyDutyButton is not null) {
            openDailyDutyButton.IsVisible = Config.ShowOpenDailyDutyButton;
        }
        
        if (dailyResetTimer is not null && Config.ShowResetTimer) {
            var nextReset = Time.NextDailyReset();
            var timeRemaining = nextReset - DateTime.UtcNow;
        
            dailyResetTimer.Text = timeRemaining.FormatTimeSpanShort(System.TimersConfig.HideTimerSeconds);
            dailyResetTimer.TextColor = Config.TimerColor;
        }
        
        if (dailyResetTimer is not null) {
            dailyResetTimer.IsVisible = Config.ShowResetTimer && addon->SelectedRadioButton == 0;
        }
                
        if (infoTextNode is not null) {
            infoTextNode.IsVisible = rouletteListController.ModifiedIndexes.Count is not 0 && addon->SelectedRadioButton is 0;
        }
    }

    private AtkComponentNode* GetListHeaderComponentNode(AddonContentsFinder* addon)
        => addon->DutyList->CategoryItemRendererList->AtkComponentListItemRenderer->ComponentNode;

    private SeString GetHintText()
        => new SeStringBuilder()
            .PushColorRgba(Config.IncompleteColor)
            .Append("Incomplete Task")
            .PopColor()
            .Append("        ")
            .PushColorRgba(Config.CompleteColor)
            .Append("Complete Task")
            .PopColor()
            .ToReadOnlySeString()
            .ToDalamudString();

    protected override void UpdateTaskLists() {
        var luminaUpdater = new LuminaTaskUpdater<ContentRoulette>(this, roulette => roulette.DutyType.ToString() != string.Empty);
        luminaUpdater.UpdateConfig(Config.TaskConfig);
        luminaUpdater.UpdateData(Data.TaskData);
    }

    public override void Update() {
        Data.TaskData.Update(ref DataChanged, rowId => InstanceContent.Instance()->IsRouletteComplete((byte) rowId));

        Data.ExpertTomestones = TryUpdateData(Data.ExpertTomestones, InventoryManager.Instance()->GetWeeklyAcquiredTomestoneCount());
        Data.ExpertTomestoneCap = TryUpdateData(Data.ExpertTomestoneCap, InventoryManager.GetLimitedTomestoneWeeklyLimit());
        Data.AtTomeCap = TryUpdateData(Data.AtTomeCap, Data.ExpertTomestones == Data.ExpertTomestoneCap);
        
        base.Update();
    }

    public override void Reset() {
        Data.TaskData.Reset();
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus() {
        if (Config.CompleteWhenCapped && Data.AtTomeCap) return ModuleStatus.Complete;

        return IncompleteTaskCount == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }
    
    protected override StatusMessage GetStatusMessage() => new LinkedStatusMessage {
        Message = $"{IncompleteTaskCount} {Strings.RoulettesRemaining}", 
        LinkEnabled = Config.ClickableLink, 
        Payload = PayloadId.OpenDutyFinderRoulette,
    };
}