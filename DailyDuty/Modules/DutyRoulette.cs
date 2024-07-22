using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using KamiLib.Classes;
using Lumina.Excel.GeneratedSheets;
using InstanceContent = FFXIVClientStructs.FFXIV.Client.Game.UI.InstanceContent;

namespace DailyDuty.Modules;

public class DutyRouletteData : ModuleTaskData<ContentRoulette> {
    public int ExpertTomestones;
    public int ExpertTomestoneCap;
    public bool AtTomeCap;

    protected override void DrawModuleData() {
        DrawDataTable([
            (Strings.CurrentWeeklyTomestones, ExpertTomestones.ToString()),
            (Strings.WeeklyTomestoneLimit, ExpertTomestoneCap.ToString()),
            (Strings.AtWeeklyTomestoneLimit, AtTomeCap.ToString()),
        ]);
    }
}

public class DutyRouletteConfig : ModuleTaskConfig<ContentRoulette> {
    public bool CompleteWhenCapped;
    public bool ClickableLink = true;
    public bool ColorContentFinder = true;
    public Vector4 CompleteColor = KnownColor.LimeGreen.Vector();
    public Vector4 IncompleteColor = KnownColor.OrangeRed.Vector();
    
    protected override bool DrawModuleConfig() {
        var configChanged = false;

        configChanged |= ImGui.Checkbox(Strings.ClickableLink, ref ClickableLink);
        configChanged |= ImGui.Checkbox(Strings.CompleteWhenTomeCapped, ref CompleteWhenCapped);
        configChanged |= ImGui.Checkbox("Color Content Finder", ref ColorContentFinder);

        if (ColorContentFinder) {
            ImGuiHelpers.ScaledDummy(5.0f);

            configChanged |= ImGuiTweaks.ColorEditWithDefault("Complete Color", ref CompleteColor, KnownColor.LimeGreen.Vector());
            configChanged |= ImGuiTweaks.ColorEditWithDefault("Incomplete Color", ref IncompleteColor, KnownColor.OrangeRed.Vector());
        }
        
        ImGuiHelpers.ScaledDummy(5.0f);
        return base.DrawModuleConfig() || configChanged;
    }
}

public unsafe class DutyRoulette : Modules.DailyTask<DutyRouletteData, DutyRouletteConfig, ContentRoulette> {
    public override ModuleName ModuleName => ModuleName.DutyRoulette;

    public override bool HasClickableLink => Config.ClickableLink;
    
    public override PayloadId ClickableLinkPayloadId => PayloadId.OpenDutyFinderRoulette;

    public override bool HasTooltip => true;

    public DutyRoulette() {
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "ContentsFinder", OnContentFinderUpdate);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, "ContentsFinder", OnContentFinderUpdate);
    }

    public override void Dispose() {
        Service.AddonLifecycle.UnregisterListener(OnContentFinderUpdate);
    }

    private void OnContentFinderUpdate(AddonEvent type, AddonArgs args) {
        if (!Config.ColorContentFinder) return;
        
        var addon = (AddonContentsFinder*) args.Addon;

        var treeListComponentNode = (AtkComponentNode*)addon->GetNodeById(52);
        if (treeListComponentNode is null) return;
        
        var treeListComponent = (AtkComponentTreeList*) treeListComponentNode->Component;
        if (treeListComponent is null) return;

        foreach (var listItem in treeListComponent->Items) {
            var listItemTextNode = listItem.Value->Renderer->ButtonTextNode;
            var listItemText = listItemTextNode->NodeText.ToString();

            var levelTextNode = (AtkTextNode*) listItem.Value->Renderer->GetTextNodeById(15);
            if (levelTextNode is null) continue;
            
            if (Service.DataManager.GetExcelSheet<ContentRoulette>()!.FirstOrDefault(rouletteData => string.Equals(listItemText, rouletteData.Category.ToString(), StringComparison.OrdinalIgnoreCase)) is {} contentRoulette){
                if (Config.TaskConfig.Any(task => task.Enabled && task.RowId == contentRoulette.RowId)) {
                    var rouletteCompleted = InstanceContent.Instance()->IsRouletteComplete((byte) contentRoulette.RowId);

                    if (rouletteCompleted) {
                        listItemTextNode->TextColor = Config.CompleteColor.ToByteColor();
                    }
                    else {
                        listItemTextNode->TextColor = Config.IncompleteColor.ToByteColor();
                    }
                }
            }
            else {
                listItemTextNode->TextColor = levelTextNode->TextColor;
            }
        }
    }
    
    protected override void UpdateTaskLists() {
        var luminaUpdater = new LuminaTaskUpdater<ContentRoulette>(this, roulette => roulette.DutyType.RawString != string.Empty);
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
    
    protected override StatusMessage GetStatusMessage() {
        var message = $"{IncompleteTaskCount} {Strings.RoulettesRemaining}";

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.OpenDutyFinderRoulette);
    }
}