using System;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using DailyDuty.System.Localization;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace DailyDuty.System;

public class MaskedCarnivaleConfig : ModuleConfigBase
{
    [SelectableTasks] 
    public LuminaTaskConfigList<Addon> TaskConfig = new();

    [ClickableLink("UldahTeleport")]
    public bool ClickableLink = true;
}

public class MaskedCarnivaleData : ModuleDataBase
{
    [SelectableTasks] 
    public LuminaTaskDataList<Addon> TaskData = new();
}

public unsafe class MaskedCarnivale : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.MaskedCarnivale;

    public override ModuleConfigBase ModuleConfig { get; protected set; } = new MaskedCarnivaleConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new MaskedCarnivaleData();
    private MaskedCarnivaleConfig Config => ModuleConfig as MaskedCarnivaleConfig ?? new MaskedCarnivaleConfig();
    private MaskedCarnivaleData Data => ModuleData as MaskedCarnivaleData ?? new MaskedCarnivaleData();

    private readonly AgentAozContentBriefing* agent = (AgentAozContentBriefing*) AgentModule.Instance()->GetAgentByInternalId(AgentId.AozContentBriefing);

    protected override void UpdateTaskLists()
    {
        var luminaTaskUpdater = new LuminaTaskUpdater<Addon>(this, addon => addon.RowId is 12449 or 12448 or 12447);
        luminaTaskUpdater.UpdateConfig(Config.TaskConfig);
        luminaTaskUpdater.UpdateData(Data.TaskData);
        
    }

    public override void Update()
    {
        if (agent is not null && agent->AgentInterface.IsAgentActive())
        {
            foreach (var task in Data.TaskData)
            {
                var status = task.RowId switch
                {
                    12449 => agent->IsWeeklyChallengeComplete(AozWeeklyChallenge.Novice),
                    12448 => agent->IsWeeklyChallengeComplete(AozWeeklyChallenge.Moderate),
                    12447 => agent->IsWeeklyChallengeComplete(AozWeeklyChallenge.Advanced),
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (task.Complete != status)
                {
                    task.Complete = status;
                    DataChanged = true;
                }
            }
        }
        
        base.Update();
    }

    public override void AddonPostSetup(AddonArgs addonInfo)
    {
        if (addonInfo.AddonName != "AOZContentResult") return;

        var atkValues = addonInfo.Addon->AtkValues;
        
        if(atkValues[112].Type != ValueType.UInt) throw new Exception("Type Mismatch Exception");
        if(atkValues[114].Type != ValueType.Bool) throw new Exception("Type Mismatch Exception");
        
        var completionIndex = atkValues[112].UInt;
        var completionStatus = atkValues[114].Byte != 0;

        var addonId = completionIndex switch
        {
            0 => 12449,
            1 => 12448,
            2 => 12447,

            _ => throw new ArgumentOutOfRangeException()
        };

        var task = Data.TaskData.FirstOrDefault(task => task.RowId == addonId);

        if (task is not null && task.Complete != completionStatus)
        {
            task.Complete = completionStatus;
            DataChanged = true;
        }
    }

    public override void Reset()
    {
        foreach (var task in Data.TaskData)
        {
            task.Complete = false;
        }
        
        base.Reset();
    }

    public override bool HasTooltip { get; protected set; } = true;
    public override string GetTooltip() => GetTaskListTooltip(Config.TaskConfig, Data.TaskData, row => LuminaCache<Addon>.Instance.GetRow(row)!.Text.ToString());

    protected override ModuleStatus GetModuleStatus() => GetIncompleteCount(Config.TaskConfig, Data.TaskData) == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override StatusMessage GetStatusMessage()
    {
        var message = $"{GetIncompleteCount(Config.TaskConfig, Data.TaskData)} {Strings.ChallengesRemaining}";

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.UldahTeleport);
    }
}