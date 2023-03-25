using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using DailyDuty.System.Localization;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class MaskedCarnivaleConfig : ModuleConfigBase
{
    [SelectableTasks] 
    public List<LuminaTaskConfig<Addon>> Tasks = new();

    [ClickableLink("UldahTeleport")]
    public bool ClickableLink = true;
}

public class MaskedCarnivaleData : ModuleDataBase
{
    [SelectableTasks] 
    public List<LuminaTaskData<Addon>> Tasks = new();
}

public unsafe class MaskedCarnivale : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.MaskedCarnivale;

    public override ModuleConfigBase ModuleConfig { get; protected set; } = new MaskedCarnivaleConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new MaskedCarnivaleData();
    private MaskedCarnivaleConfig Config => ModuleConfig as MaskedCarnivaleConfig ?? new MaskedCarnivaleConfig();
    private MaskedCarnivaleData Data => ModuleData as MaskedCarnivaleData ?? new MaskedCarnivaleData();

    private readonly AgentAozContentBriefing* agent = (AgentAozContentBriefing*) AgentModule.Instance()->GetAgentByInternalId(AgentId.AozContentBriefing);

    public override void Load()
    {
        base.Load();

        var luminaTaskUpdater = new LuminaTaskUpdater<Addon>(this, addon => addon.RowId is 12449 or 12448 or 12447);
        luminaTaskUpdater.UpdateConfig(Config.Tasks);
        luminaTaskUpdater.UpdateData(Data.Tasks);
    }

    public override void Update()
    {
        if (agent is not null && agent->AgentInterface.IsAgentActive())
        {
            foreach (var task in Data.Tasks)
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

        var completionIndex = addonInfo.Addon->AtkValues[109].Int;
        var completionStatus = addonInfo.Addon->AtkValues[111].Byte != 0;

        if (completionIndex < Data.Tasks.Count)
        {
            if (Data.Tasks[completionIndex].Complete != completionStatus)
            {
                Data.Tasks[completionIndex].Complete = completionStatus;
                DataChanged = true;
            }
        }
    }

    public override void Reset()
    {
        foreach (var task in Data.Tasks)
        {
            task.Complete = false;
        }
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus() => GetIncompleteCount(Config.Tasks, Data.Tasks) == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override StatusMessage GetStatusMessage()
    {
        var message = $"{GetIncompleteCount(Config.Tasks, Data.Tasks)} {Strings.ChallengesRemaining}";

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.UldahTeleport);
    }
}