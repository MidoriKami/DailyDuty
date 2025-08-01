﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using DailyDuty.Windows;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.Extensions;
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

    private Hook<AtkComponentListItemPopulator.PopulateDelegate>? onDutyListPopulate;
    private readonly List<uint> modifiedIndexes = [];
    
    public DutyRoulette() {
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "ContentsFinder", OnContentsFinderSetup); 
        Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "ContentsFinder", OnContentsFinderFinalize);
        
        System.ContentsFinderController.OnAttach += AttachNodes;
        System.ContentsFinderController.OnDetach += DetachNodes;
        System.ContentsFinderController.OnUpdate += OnContentFinderUpdate;
    }

    public override void Dispose() {
        Service.AddonLifecycle.UnregisterListener(OnContentsFinderSetup, OnContentsFinderFinalize);
        
        System.ContentsFinderController.OnAttach -= AttachNodes;
        System.ContentsFinderController.OnDetach -= DetachNodes;
        System.ContentsFinderController.OnUpdate -= OnContentFinderUpdate;
        
        onDutyListPopulate?.Dispose();
        
        base.Dispose();
    }
    
    private void OnContentsFinderSetup(AddonEvent type, AddonArgs args) {
        var addon = args.GetAddon<AddonContentsFinder>();
        var populateMethod = addon->DutyList->GetItemRendererByNodeId(6)->Populator.Populate;

        onDutyListPopulate = Service.Hooker.HookFromAddress<AtkComponentListItemPopulator.PopulateDelegate>(populateMethod, OnPopulateHook);
        onDutyListPopulate?.Enable();
    }
    
    private void OnContentsFinderFinalize(AddonEvent type, AddonArgs args) {
        onDutyListPopulate?.Dispose();
        modifiedIndexes.Clear();
    }

    private void OnPopulateHook(AtkUnitBase* unitBase, AtkComponentListItemPopulator.ListItemInfo* listItemInfo, AtkResNode** nodeList) => HookSafety.ExecuteSafe(() => {
        var index = listItemInfo->ListItem->Renderer->OwnerNode->NodeId;
        
        var dutyName = listItemInfo->ListItem->StringValues[0].ToString();
        var dutyInfo = Service.DataManager.GetExcelSheet<ContentRoulette>().FirstOrNull(roulette => string.Equals(dutyName, roulette.Category.ExtractText(), StringComparison.OrdinalIgnoreCase));

        var dutyNameTextNode = (AtkTextNode*) nodeList[3];
        var levelTextNode = (AtkTextNode*) nodeList[4];
        
        // If this is already modified
        if (modifiedIndexes.Contains(index)) {
            // And is not a roulette, restore the original color
            if (dutyInfo is null) {
                dutyNameTextNode->TextColor = levelTextNode->TextColor;
                modifiedIndexes.Remove(index);
            }
        }
        // else, this hasn't been modified, and is a roulette
        else if (dutyInfo is { } roulette) {
            // If this roulette is being tracked
            var taskSettings = Config.TaskConfig.FirstOrDefault(task => task.RowId == roulette.RowId);
            if (taskSettings is { Enabled: true }) {
                var isRouletteCompleted = InstanceContent.Instance()->IsRouletteComplete((byte) roulette.RowId);
                
                // Color it appropriately
                if (isRouletteCompleted) {
                    dutyNameTextNode->TextColor = Config.CompleteColor.ToByteColor();
                }
                else {
                    dutyNameTextNode->TextColor = Config.IncompleteColor.ToByteColor();
                }

                // Track that this node has been modified
                modifiedIndexes.Add(index);
            }
        }
        
        if (infoTextNode is not null) {
            infoTextNode.IsVisible = modifiedIndexes.Count is not 0;
        }
    
        onDutyListPopulate!.Original(unitBase, listItemInfo, nodeList);
    }, Service.Log);

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
        System.NativeController.DetachNode(infoTextNode, () => {
            infoTextNode?.Dispose();
            infoTextNode = null;
        });
        
        System.NativeController.DetachNode(openDailyDutyButton, () => {
            openDailyDutyButton?.Dispose();
            openDailyDutyButton = null;
        });
        
        System.NativeController.DetachNode(dailyResetTimer, () => {
            dailyResetTimer?.Dispose();
            dailyResetTimer = null;
        });
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