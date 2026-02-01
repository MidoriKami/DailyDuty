using System;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Agent;
using Dalamud.Game.Agent.AgentArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using Lumina.Extensions;
using Lumina.Text.ReadOnly;
using AgentId = Dalamud.Game.Agent.AgentId;

namespace DailyDuty.Features.GrandCompanySquadron;

/// <summary>
/// This module needs to be completely redesigned.
/// </summary>
public unsafe class GrandCompanySquadron : Module<ConfigBase, GrandCompanySquadronData> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Grand Company Squadron",
        FileName = "GrandCompanySquadron",
        Type = ModuleType.Weekly,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "GrandCompany", "GC", "Gil", "Company Seals", "Seals" ],
    };

    public override DataNodeBase DataNode => new GrandCompanySquadronDataNode(this);

    protected override void OnModuleEnable() {
        Services.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "GcArmyExpeditionResult", GcArmyExpeditionResultPreFinalize);
        Services.AgentLifecycle.RegisterListener(AgentEvent.PreReceiveEvent, AgentId.GcArmyExpedition, AgentGcArmyExpeditionReceiveEvent);
    }

    protected override void OnModuleDisable() {
        Services.AddonLifecycle.UnregisterListener(GcArmyExpeditionResultPreFinalize);
        Services.AgentLifecycle.UnregisterListener(AgentGcArmyExpeditionReceiveEvent);
    }

    protected override StatusMessage GetStatusMessage() {
        if (ModuleData.MissionStarted && DateTime.UtcNow >= ModuleData.MissionCompleteTime) return "Mission Results Ready";

        return "Mission Not Started";
    }

    public override DateTime GetNextResetDateTime()
        => Time.NextWeeklyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    public override void Reset()
        => ModuleData.MissionCompleted = false;

    protected override CompletionStatus GetCompletionStatus() {
        if (ModuleData.MissionCompleted) return CompletionStatus.Complete;
        if (ModuleData.MissionStarted) return CompletionStatus.InProgress;

        return CompletionStatus.Incomplete;
    }
    
    private void GcArmyExpeditionResultPreFinalize(AddonEvent type, AddonArgs args) {
        // Note: there may be a NPCInteract Scene that includes the specific data needed to do this much more efficiently.
        // But I have to wait 18 hours to even check :skull:
        
        ModuleData.MissionStarted = false;
        ModuleData.MissionCompleteTime = DateTime.MinValue;
        ModuleData.MarkDirty();

        if (args.Addon.AtkValues.ElementAt(4).TryGet(out ReadOnlySeString? missionName)) return;
        if (args.Addon.AtkValues.ElementAt(2).TryGet(out int? missionSuccessful) || missionSuccessful is not 1) return;

        var missionData = Services.DataManager.GetExcelSheet<GcArmyExpedition>()
            .FirstOrNull(entry => entry.Name == missionName);

        // Logging in-case this breaks for whatever reason
        if (missionData is null) {
            Services.PluginLog.Debug($"Failed to find GcArmyExpedition, {missionName}");
        }

        if (missionData?.GcArmyExpeditionType.RowId is not 3) return; // Completed Non-Priority Mission

        ModuleData.MissionCompleted = true;
        ModuleData.MarkDirty();
    }
    
    private void AgentGcArmyExpeditionReceiveEvent(AgentEvent type, AgentArgs args) {
        if (args is not AgentReceiveEventArgs eventArgs) return;
        if (eventArgs.EventKind is not 1) return; // SelectYesNo Callback
        if (eventArgs.AtkValueSpan[0].Int is not 0) return; // Yes selected

        ModuleData.MissionStarted = true;
        ModuleData.MissionCompleteTime = DateTime.UtcNow.AddHours(18);
        ModuleData.MarkDirty();
    }

    public override void OnNpcInteract(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) {
        if (gameObject->BaseId is not 0xF845C) return;
        if (scene is not 1) return;
        if (sceneDataCount is not 7) return;

        if (sceneData[3] is 0) {
            ModuleData.MissionStarted = false;
            ModuleData.MissionCompleteTime = DateTime.MinValue;
            ModuleData.MarkDirty();
            return;
        }

        var missionData = Services.DataManager.GetExcelSheet<GcArmyExpedition>().GetRow(sceneData[2]);
        var isPriorityMission = missionData.GcArmyExpeditionType.RowId is 3;

        if (isPriorityMission) {
            ModuleData.MissionStarted = isPriorityMission;
            ModuleData.MissionCompleteTime = DateTime.UtcNow + TimeSpan.FromSeconds(sceneData[3]);
        }
        else {
            ModuleData.MissionStarted = false;
        }
        
        ModuleData.MarkDirty();
    }

    protected override void OnModuleUpdate() {
        var gcAgent = AgentGcArmyExpedition.Instance();
		
        if (gcAgent->IsAgentActive() && gcAgent->SelectedTab is 2) {
            if (ModuleData.MissionCompleted != gcAgent->ExpeditionData->MissionInfo[0].Available is 0) {
                ModuleData.MissionCompleted = gcAgent->ExpeditionData->MissionInfo[0].Available is 0;
                ModuleData.MarkDirty();
            }
        }

        if (ModuleData.MissionCompleteTime > DateTime.UtcNow) {
            ModuleData.TimeUntilMissionComplete = ModuleData.MissionCompleteTime - DateTime.UtcNow;
        }
        else {
            ModuleData.TimeUntilMissionComplete = TimeSpan.MinValue;
        }
    }
}
