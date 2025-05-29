using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using DailyDuty.Windows;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using KamiLib.Classes;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.ComponentNodes;
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
    
    public DutyRoulette() {
        System.ContentsFinderController.OnAttach += AttachNodes;
        System.ContentsFinderController.OnDetach += DetachNodes;
        System.ContentsFinderController.OnUpdate += OnContentFinderUpdate;
        System.ContentsFinderController.OnRefresh += OnContentFinderUpdate;
    }

    public override void Dispose() {
        System.ContentsFinderController.OnAttach -= AttachNodes;
        System.ContentsFinderController.OnDetach -= DetachNodes;
        System.ContentsFinderController.OnUpdate -= OnContentFinderUpdate;
        System.ContentsFinderController.OnRefresh -= OnContentFinderUpdate;
        
        base.Dispose();
    }

    private void AttachNodes(AddonContentsFinder* addon) {
        if (addon is null) return;

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
        };
        
        System.NativeController.AttachToAddon(infoTextNode, addon, targetResNode, NodePosition.AfterTarget);
        
        openDailyDutyButton = new TextButtonNode {
            Position = new Vector2(50.0f, 622.0f),
            Size = new Vector2(130.0f, 28.0f),
            IsVisible = true,
            Label = "Open DailyDuty",
        };
        
        openDailyDutyButton.AddEvent(AddonEventType.ButtonClick, () => System.WindowManager.GetWindow<ConfigurationWindow>()?.UnCollapseOrToggle() );

        System.NativeController.AttachToAddon(openDailyDutyButton, addon, addon->RootNode, NodePosition.AsLastChild);

        dailyResetTimer = new TextNode {
            Position = new Vector2(300.0f, 202.0f),
            Size = new Vector2(148.0f, 24.0f),
            AlignmentType = AlignmentType.Right,
            Tooltip = "Time until next daily reset",
            Text = "0:00:00:00",
            EnableEventFlags = true,
            TextColor = Config.TimerColor == Vector4.Zero ? ColorHelper.GetColor(1) : Config.TimerColor,
        };

        System.NativeController.AttachToAddon(dailyResetTimer, addon, addon->RootNode, NodePosition.AsLastChild);
    }

    private void DetachNodes(AddonContentsFinder* addon) {
        System.NativeController.DetachFromAddon(infoTextNode, addon, () => {
            infoTextNode?.Dispose();
            infoTextNode = null;
        });
        
        System.NativeController.DetachFromAddon(openDailyDutyButton, addon, () => {
            openDailyDutyButton?.Dispose();
            openDailyDutyButton = null;
        });
        
        System.NativeController.DetachFromAddon(dailyResetTimer, addon, () => {
            dailyResetTimer?.Dispose();
            dailyResetTimer = null;
        });
    }

    private void OnContentFinderUpdate(AddonContentsFinder* addon) {
        if (openDailyDutyButton is not null) {
            openDailyDutyButton.IsVisible = Config.ShowOpenDailyDutyButton;
        }
        
        if (infoTextNode is not null) {
            infoTextNode.IsVisible = false;
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
        
        if (!Config.ColorContentFinder) return;
        if (!Config.ModuleEnabled) return;
        
        var treeListComponentNode = (AtkComponentNode*)addon->GetNodeById(52);
        if (treeListComponentNode is null) return;
        
        var treeListComponent = (AtkComponentTreeList*) treeListComponentNode->Component;
        if (treeListComponent is null) return;

        var anyRecolored = false;

        foreach (var listItem in treeListComponent->Items) {
            if (listItem.Value->Renderer is null) continue;

            var listItemTextNode = (AtkTextNode*) listItem.Value->Renderer->GetTextNodeById(5);
            if (listItemTextNode is null) continue;

            var listItemText = listItemTextNode->NodeText.ToString();

            var levelTextNode = (AtkTextNode*) listItem.Value->Renderer->GetTextNodeById(18);
            if (levelTextNode is null) continue;

            foreach (var roulette in Service.DataManager.GetExcelSheet<ContentRoulette>()) {
                if (roulette.RowId is 0) continue;
                
                var rouletteString = roulette.Category.ExtractText();

                if (string.Equals(listItemText, rouletteString)) {
                    var taskSettings = Config.TaskConfig.FirstOrDefault(task => task.RowId == roulette.RowId);
                    if (taskSettings is { Enabled: true }) {
                        var isRouletteCompleted = InstanceContent.Instance()->IsRouletteComplete((byte) roulette.RowId);
                        if (isRouletteCompleted) {
                            listItemTextNode->TextColor = Config.CompleteColor.ToByteColor();
                        }
                        else {
                            listItemTextNode->TextColor = Config.IncompleteColor.ToByteColor();
                        }

                        anyRecolored = true;
                    }

                    break;
                }
            }

            if (!anyRecolored) {
                listItemTextNode->TextColor = levelTextNode->TextColor;
            }
        }

        if (infoTextNode is not null) {
            infoTextNode.IsVisible = anyRecolored;
            infoTextNode.Text = GetHintText();
        }
    }

    private SeString GetHintText()
        => new SeStringBuilder()
            .PushColorRgba(Config.IncompleteColor)
            .Append("Incomplete Task")
            .PopColor()
            .Append("        ")
            .PushColorRgba(Config.CompleteColor)
            .Append("Complete Task")
            .PopColor()
            .ToSeString()
            .ToDalamudString();

    protected override void UpdateTaskLists() {
        var luminaUpdater = new LuminaTaskUpdater<ContentRoulette>(this, roulette => roulette.DutyType.ExtractText() != string.Empty);
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