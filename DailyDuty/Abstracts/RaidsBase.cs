using System.Collections.Generic;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Interfaces;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class RaidsBaseConfig : ModuleConfigBase
{
    [SelectableTasks]
    public LuminaTaskConfigList<ContentFinderCondition> TaskConfig = new();

    [ClickableLink("OpenDutyFinderToRaid")]
    public bool ClickableLink = true;
}

public class RaidsBaseData : ModuleDataBase
{
    [SelectableTasks] 
    public LuminaTaskDataList<ContentFinderCondition> TaskData = new();
}

public abstract unsafe class RaidsBase : Module.WeeklyModule, IChatMessageReceiver
{
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new RaidsBaseConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new RaidsBaseData();
    
    protected RaidsBaseConfig Config => ModuleConfig as RaidsBaseConfig ?? new RaidsBaseConfig();
    protected RaidsBaseData Data => ModuleData as RaidsBaseData ?? new RaidsBaseData();

    protected override ModuleStatus GetModuleStatus() => GetIncompleteCount(Config.TaskConfig, Data.TaskData) == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    private AgentContentsFinder* Agent => (AgentContentsFinder*) AgentModule.Instance()->GetAgentByInternalId(AgentId.ContentsFinder);
    
    public override void Update()
    {
        if (Agent is not null && Agent->AgentInterface.IsAgentActive())
        {
            var selectedDuty = Agent->SelectedDutyId;
            var task = Data.TaskData.FirstOrDefault(task => task.RowId == selectedDuty);
            var numRewards = Agent->NumCollectedRewards;
            
            if (task is not null && task.CurrentCount != numRewards)
            {
                task.CurrentCount = numRewards;
                DataChanged = true;
            }
        }
        
        base.Update();
    }

    public override void Reset()
    {
        Data.TaskData.Reset();
        
        base.Reset();
    }

    public void OnChatMessage(XivChatType type, uint senderID, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        // If message is a loot message
        if (((int)type & 0x7F) != 0x3E) return;

        // If we are in a zone that we are tracking
        if (GetDataForCurrentZone() is not { } trackedRaid) return;

        // If the message does NOT contain a player payload
        if (message.Payloads.FirstOrDefault(p => p is PlayerPayload) is PlayerPayload) return;

        // If the message DOES contain an item
        if (message.Payloads.FirstOrDefault(p => p is ItemPayload) is not ItemPayload { Item: { } item } ) return;

        switch (item.ItemUICategory.Row)
        {
            case 34: // Head
            case 35: // Body
            case 36: // Legs
            case 37: // Hands
            case 38: // Feet
            case 61 when item.ItemAction.Row == 0: // Miscellany with no itemAction
                trackedRaid.CurrentCount += 1;
                DataChanged = true;
                break;
        }
    }
    
    private LuminaTaskData<ContentFinderCondition>? GetDataForCurrentZone()
    {
        return (
            from task in Data.TaskData 
            let cfcData = LuminaCache<ContentFinderCondition>.Instance.GetRow(task.RowId)! 
            where cfcData.TerritoryType.Row == Service.ClientState.TerritoryType 
            select task
            ).FirstOrDefault();
    }
    
    private bool IsDataStale(ICollection<uint> dutyList) => Data.TaskData.Any(task => !dutyList.Contains(
        LuminaCache<ContentFinderCondition>.Instance.GetRow(task.RowId)!.TerritoryType.Row)
    );
    
    protected void CheckForDutyListUpdate(List<uint> dutyList)
    {
        if (IsDataStale(dutyList) || !Config.TaskConfig.Any() || !Data.TaskData.Any())
        {
            Config.TaskConfig.Clear();
            Data.TaskData.Clear();

            foreach (var duty in dutyList)
            {
                var cfc = LuminaCache<ContentFinderCondition>.Instance.First(entry => entry.TerritoryType.Row == duty);
                
                Config.TaskConfig.Add(new LuminaTaskConfig<ContentFinderCondition>
                {
                    RowId = cfc.RowId,
                    Enabled = false,
                    TargetCount = 0
                });
                
                Data.TaskData.Add(new LuminaTaskData<ContentFinderCondition>
                {
                    RowId = cfc.RowId,
                    Complete = false,
                    CurrentCount = 0
                });
                
                SaveConfig();
                SaveData();
            }
        }
    }
    
    public override bool HasTooltip { get; protected set; } = true;
    public override string GetTooltip() => GetTaskListTooltip(Config.TaskConfig, Data.TaskData, row => LuminaCache<ContentFinderCondition>.Instance.GetRow(row)!.Name.ToString());
}