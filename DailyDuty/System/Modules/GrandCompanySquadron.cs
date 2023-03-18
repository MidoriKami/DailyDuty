using System.Collections.Generic;
using System.Text.RegularExpressions;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Caching;
using KamiLib.Hooking;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class GrandCompanySquadronConfig : ModuleConfigBase
{
    // No Config Options for this Module
}

public class GrandCompanySquadronData : ModuleDataBase
{
    [DataDisplay("MissionCompleted")]
    public bool MissionCompleted;

    [DataDisplay("MissionStarted")]
    public bool MissionStarted;
}

public unsafe partial class GrandCompanySquadron : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.GrandCompanySquadron;

    public override ModuleConfigBase ModuleConfig { get; protected set; } = new GrandCompanySquadronConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new GrandCompanySquadronData();
    private GrandCompanySquadronData Data => ModuleData as GrandCompanySquadronData ?? new GrandCompanySquadronData();
    
    private Hook<Delegates.Agent.ReceiveEvent>? onReceiveEventHook;
    private AgentGcArmyExpedition* Agent => (AgentGcArmyExpedition*) AgentModule.Instance()->GetAgentByInternalId(AgentId.GcArmyExpedition);

    [GeneratedRegex("[^\\p{L}\\p{N}]")]
    private static partial Regex Alphanumeric();
    
    private readonly Dictionary<string, GcArmyExpedition> missionLookup = new();
    private readonly Addon? missionComplete;

    public GrandCompanySquadron()
    {
        foreach (var mission in LuminaCache<GcArmyExpedition>.Instance)
        {
            missionLookup.TryAdd(Alphanumeric().Replace(mission.Name.RawString.ToLower(), string.Empty), mission);
        }

        missionComplete = LuminaCache<Addon>.Instance.GetRow(10572); // "Mission Complete!"
    }
    
    public override void Load()
    {
        base.Load();

        onReceiveEventHook ??= Hook<Delegates.Agent.ReceiveEvent>.FromAddress(new nint(Agent->AgentInterface.VTable->ReceiveEvent), OnReceiveEvent);
        onReceiveEventHook?.Enable();
    }

    // The mission is no longer in progress when the window closes
    public override void AddonFinalize(SetupAddonArgs addonInfo)
    {
        if (addonInfo.AddonName != "GcArmyExpeditionResult") return;

        Data.MissionStarted = false;
        DataChanged = true;
        
        var missionTextNode = addonInfo.Addon->GetTextNodeById(4);
        var missionResultTextNode = addonInfo.Addon->GetTextNodeById(8);
        if (missionTextNode is not null && missionResultTextNode is not null)
        {
            var missionText = Alphanumeric().Replace(missionTextNode->NodeText.ToString(), string.Empty);

            if (missionLookup.TryGetValue(missionText, out var missionInfo))
            {
                var resultText = missionResultTextNode->NodeText.ToString();
                var missionCompleteText = missionComplete?.Text.ToString();
                var missionCompleted = resultText == missionCompleteText;

                const int weeklyMissionType = 3;
                var isWeeklyMission = missionInfo.GcArmyExpeditionType.Row == weeklyMissionType;
                
                if (isWeeklyMission && missionCompleted)
                {
                    Data.MissionCompleted = true;
                    DataChanged = true;
                }
            }
        }
    }

    public override void Unload()
    {
        base.Unload();
        
        onReceiveEventHook?.Disable();
    }

    public override void Dispose()
    {
        onReceiveEventHook?.Dispose();
    }

    public override void Reset()
    {
        Data.MissionCompleted = false;
        Data.MissionStarted = false;
        
        base.Reset();
    }

    public override void Update()
    {
        if (Agent is not null && Agent->AgentInterface.IsAgentActive() && Agent->SelectedTab == 2)
        {
            var completed = Agent->ExpeditionData->MissionInfoArraySpan[0].Available == 0;
        
            if (Data.MissionCompleted != completed)
            {
                Data.MissionCompleted = completed;
                DataChanged = true;
            }
        }
        
        base.Update();
    }
    
    private nint OnReceiveEvent(AgentInterface* agent, nint rawData, AtkValue* args, uint argCount, ulong sender)
    {
        var result = onReceiveEventHook!.Original(agent, rawData, args, argCount, sender);
        
        Safety.ExecuteSafe(() =>
        {
            if (sender == 1 && args[0].Int == 0)
            {
                Data.MissionStarted = true;
                DataChanged = true;
            }
        });

        return result;
    }

    protected override ModuleStatus GetModuleStatus()
    {
        if (Data.MissionStarted) return ModuleStatus.InProgress;
        
        return Data.MissionCompleted ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = "Mission Available",
    };
}