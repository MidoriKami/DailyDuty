using System;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Agent;
using Dalamud.Game.Agent.AgentArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiToolKit.Controls.Builders;
using KamiToolKit.UI.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using AgentId = Dalamud.Game.Agent.AgentId;

namespace DailyDuty.Modules;

public class GrandCompanySquadron : Module<GrandCompanySquadronData, GrandCompanySquadronConfig>, IWeeklyResettable, IFrameworkUpdatable, INpcInteractListener {

    public override string DisplayName => Strings.GrandCompanySquadron_DisplayName;

    public override GrandCompanySquadronData Data { get; protected set; } = null!;
    public override GrandCompanySquadronConfig Config { get; protected set; } = null!;

    private TextNode? missionStatusNode;
    private TextNode? completionTimeNode;

    private ChatLinkPayload? grandCompanyTeleportPayload;


    public override async Task EnableAsync() {
        Data = await GrandCompanySquadronData.Load();
        Config = await GrandCompanySquadronConfig.Load();

        grandCompanyTeleportPayload = ChatLinkPayload.TeleportToGrandCompany();

        IAddonLifecycle.Get().RegisterListener(AddonEvent.PreFinalize, "GcArmyExpeditionResult", GcArmyExpeditionResultPreFinalize);
        IAgentLifecycle.Get().RegisterListener(AgentEvent.PreReceiveEvent, AgentId.GcArmyExpedition, AgentGcArmyExpeditionReceiveEvent);
    }

    public override Task DisableAsync() {
        IAddonLifecycle.Get().UnregisterListener(GcArmyExpeditionResultPreFinalize);
        IAgentLifecycle.Get().UnregisterListener(AgentGcArmyExpeditionReceiveEvent);

        grandCompanyTeleportPayload?.Dispose();
        grandCompanyTeleportPayload = null;

        missionStatusNode = null;
        completionTimeNode = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .DataRow(Strings.GrandCompanySquadron_MissionCompleted, Data.MissionCompleted.ToString(), out missionStatusNode)
        .DataRow(Strings.GrandCompanySquadron_CompletionTime, GetCompletionTimeString(), out completionTimeNode);

    public override void UpdateDataUi() {
        missionStatusNode?.String = Data.MissionCompleted ? Strings.CompletionStatus_Complete : Data.MissionStarted ? Strings.CompletionStatus_InProgress : Strings.CompletionStatus_Incomplete;
        completionTimeNode?.String = GetCompletionTimeString();
    }

    public unsafe void Update() {
        var gcAgent = AgentGcArmyExpedition.Instance();

        if (gcAgent->IsAgentActive() && gcAgent->SelectedTab is 2) {
            var completed = gcAgent->ExpeditionData->MissionInfo[0].Available is 0;
            if (Data.MissionCompleted != completed) {
                Data.MissionCompleted = completed;
                Data.Save();
            }
        }
    }

    protected override Task ResetAsync() {
        Data.MissionCompleted = false;
        Data.MissionStarted = false;
        Data.MissionCompleteTime = DateTime.MinValue;
        Data.Save();
        return Task.CompletedTask;
    }

    protected override CompletionStatus GetCompletionStatus() {
        if (Data.MissionCompleted) return CompletionStatus.Complete;
        if (Data.MissionStarted && DateTime.UtcNow < Data.MissionCompleteTime) return CompletionStatus.InProgress;
        if (Data.MissionStarted && DateTime.UtcNow >= Data.MissionCompleteTime) return CompletionStatus.ResultsAvailable;

        return CompletionStatus.Incomplete;
    }

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        grandCompanyTeleportPayload,
        DisplayName,
        message ?? GetCompletionStatus() switch {
            CompletionStatus.Complete => Strings.StatusMessages_MissionResultsReady,
            CompletionStatus.InProgress => Strings.GrandCompanySquadron_MissionStarted,
            _ => Strings.StatusMessages_MissionNotStarted,
        }
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        grandCompanyTeleportPayload,
        DisplayName,
        message ?? "Module has been reset."
    );

    private void GcArmyExpeditionResultPreFinalize(AddonEvent type, AddonArgs args) {
        Data.MissionStarted = false;
        Data.MissionCompleteTime = DateTime.MinValue;
        Data.Save();

        var atkValues = args.Addon.AtkValueSpan;
        if (!atkValues[4].TryGet(out ReadOnlySeString missionName)) return;
        if (!atkValues[2].TryGet(out int missionSuccessful) || missionSuccessful is not 1) return;

        GcArmyExpedition? missionData = null;
        foreach (var entry in IDataManager.Get().GetExcelSheet<GcArmyExpedition>()) {
            if (entry.Name == missionName) {
                missionData = entry;
                break;
            }
        }

        if (missionData is null) {
            IPluginLog.Get().Debug($"Failed to find GcArmyExpedition, {missionName}");
            return;
        }

        if (missionData.Value.GcArmyExpeditionType.RowId is not 3) return;

        Data.MissionCompleted = true;
        Data.Save();
    }

    private void AgentGcArmyExpeditionReceiveEvent(AgentEvent type, AgentArgs args) {
        if (args is not AgentReceiveEventArgs eventArgs) return;
        if (eventArgs.EventKind is not 1) return;

        var atkValues = eventArgs.AtkValueSpan;
        if (atkValues.Length < 1) return;
        if (!atkValues[0].TryGet(out int value) || value is not 0) return;

        Data.MissionStarted = true;
        Data.MissionCompleteTime = DateTime.UtcNow.AddHours(18);
        Data.Save();
    }

    public unsafe void OnNpcInteract(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) {
        if (gameObject->BaseId is not 1015468) return;
        if (scene is not 1) return;
        if (sceneDataCount is not 7) return;

        if (sceneData[3] is 0) {
            Data.MissionStarted = false;
            Data.MissionCompleteTime = DateTime.MinValue;
            Data.Save();
            return;
        }

        var missionData = IDataManager.Get().GetExcelSheet<GcArmyExpedition>().GetRow(sceneData[2]);
        var isPriorityMission = missionData.GcArmyExpeditionType.RowId is 3;

        if (isPriorityMission) {
            Data.MissionStarted = true;
            Data.MissionCompleteTime = DateTime.UtcNow + TimeSpan.FromSeconds(sceneData[3]);
        }
        else {
            Data.MissionStarted = false;
        }

        Data.Save();
    }

    private string GetCompletionTimeString() {
        if (!Data.MissionStarted || Data.MissionCompleteTime == DateTime.MinValue) {
            return Strings.GrandCompanySquadron_NotStarted;
        }

        var timeRemaining = Data.MissionCompleteTime - DateTime.UtcNow;
        if (timeRemaining.TotalSeconds < 0) {
            return Strings.StatusMessages_MissionResultsReady;
        }

        return $"{timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";
    }
}

public class GrandCompanySquadronData : ModuleDataFile<GrandCompanySquadronData> {
    protected override string FileName => "GrandCompanySquadron.data.json";

    public bool MissionCompleted { get; set; }
    public bool MissionStarted { get; set; }
    public DateTime MissionCompleteTime { get; set; } = DateTime.MinValue;
}

public class GrandCompanySquadronConfig : ModuleConfigFile<GrandCompanySquadronConfig> {
    protected override string FileName => "GrandCompanySquadron.config.json";
}
