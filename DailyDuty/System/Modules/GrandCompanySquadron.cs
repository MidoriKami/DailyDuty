using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.Models.ModuleData;
using DailyDuty.System.Localization;
using Dalamud.Game.Addon;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Atk;
using KamiLib.Caching;
using KamiLib.Hooking;
using Lumina.Excel.GeneratedSheets;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace DailyDuty.System;

public unsafe partial class GrandCompanySquadron : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.GrandCompanySquadron;

    public override IModuleConfigBase ModuleConfig { get; protected set; } = new GrandCompanySquadronConfig();
    public override IModuleDataBase ModuleData { get; protected set; } = new GrandCompanySquadronData();
    private GrandCompanySquadronData Data => ModuleData as GrandCompanySquadronData ?? new GrandCompanySquadronData();
    
    private Hook<Delegates.AgentReceiveEvent>? onReceiveEventHook;
    private AgentGcArmyExpedition* Agent => (AgentGcArmyExpedition*) AgentModule.Instance()->GetAgentByInternalId(AgentId.GcArmyExpedition);

    [GeneratedRegex("[^\\p{L}\\p{N}]")]
    private static partial Regex Alphanumeric();
    
    private readonly Dictionary<string, GcArmyExpedition> missionLookup = new();
    
    public GrandCompanySquadron()
    {
        foreach (var mission in LuminaCache<GcArmyExpedition>.Instance)
        {
            missionLookup.TryAdd(Alphanumeric().Replace(mission.Name.RawString.ToLower(), string.Empty), mission);
        }
    }
    
    public override void Load()
    {
        base.Load();

        Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "GcArmyExpeditionResult", GcArmyExpeditionResultPreFinalize);
        
        onReceiveEventHook ??= Service.Hooker.HookFromAddress<Delegates.AgentReceiveEvent>(new nint(Agent->AgentInterface.VTable->ReceiveEvent), OnReceiveEvent);
        onReceiveEventHook?.Enable();
    }

    // The mission is no longer in progress when the window closes
    public void GcArmyExpeditionResultPreFinalize(AddonEvent eventType, AddonArgs addonInfo)
    {
        var addon = (AtkUnitBase*) addonInfo.Addon;
        
        Data.MissionStarted = false;
        DataChanged = true;

        if (addon->AtkValues[4].Type is not ValueType.String) throw new Exception("Type Mismatch Exception");
        if (addon->AtkValues[2].Type is not ValueType.Int) throw new Exception("Type Mismatch Exception");
        
        var missionText = Alphanumeric().Replace(addon->AtkValues[4].GetString().ToLower(), string.Empty);
        var missionSuccessful = addon->AtkValues[2].Int == 1;

        var missionInfo = LuminaCache<GcArmyExpedition>.Instance
            .FirstOrDefault(mission => Alphanumeric().Replace(mission.Name.ToString().ToLower(), string.Empty) == missionText);

        if (missionInfo is { GcArmyExpeditionType.Row: 3 } && missionSuccessful)
        {
            Data.MissionCompleted = true;
            DataChanged = true;
        }
    }

    public override void Unload()
    {
        base.Unload();
        
        Service.AddonLifecycle.UnregisterListener(GcArmyExpeditionResultPreFinalize);
        
        onReceiveEventHook?.Disable();
    }

    public override void Dispose()
    {
        onReceiveEventHook?.Dispose();
    }

    public override void Reset()
    {
        Data.MissionCompleted = false;
        
        base.Reset();
    }

    public override void Update()
    {
        if (Agent is not null && Agent->AgentInterface.IsAgentActive() && Agent->SelectedTab == 2)
        {
            Data.MissionCompleted = TryUpdateData(Data.MissionCompleted, Agent->ExpeditionData->MissionInfoArraySpan[0].Available == 0);
        }

        if (Data.MissionCompleteTime > DateTime.UtcNow)
        {
            Data.TimeUntilMissionComplete = Data.MissionCompleteTime - DateTime.UtcNow;
        }
        else
        {
            Data.TimeUntilMissionComplete = TimeSpan.Zero;
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
                var missionCompleteDateTime = DateTime.UtcNow + TimeSpan.FromHours(18);
                Data.MissionCompleteTime = new DateTime(
                    missionCompleteDateTime.Year,
                    missionCompleteDateTime.Month,
                    missionCompleteDateTime.Day,
                    missionCompleteDateTime.Hour,
                    missionCompleteDateTime.Minute,
                    missionCompleteDateTime.Second,
                    Data.NextReset.Millisecond,
                    Data.NextReset.Microsecond
                );
                DataChanged = true;
            }
        });

        return result;
    }

    protected override ModuleStatus GetModuleStatus()
    {
        if (Data.MissionStarted && Data.TimeUntilMissionComplete != TimeSpan.Zero) return ModuleStatus.InProgress;
        
        return Data.MissionCompleted ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = Data.MissionStarted && Data.TimeUntilMissionComplete == TimeSpan.Zero ? Strings.MissionCompleted : Strings.MissionAvailable
    };
}