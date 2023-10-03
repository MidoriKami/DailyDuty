using System;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using DailyDuty.System.Localization;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace DailyDuty.System;

public unsafe class MaskedCarnivale : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.MaskedCarnivale;

    public override IModuleConfigBase ModuleConfig { get; protected set; } = new MaskedCarnivaleConfig();
    public override IModuleDataBase ModuleData { get; protected set; } = new ModuleTaskDataBase<Addon>();
    private MaskedCarnivaleConfig Config => ModuleConfig as MaskedCarnivaleConfig ?? new MaskedCarnivaleConfig();
    private ModuleTaskDataBase<Addon> Data => ModuleData as ModuleTaskDataBase<Addon> ?? new ModuleTaskDataBase<Addon>();

    private readonly AgentAozContentBriefing* agent = (AgentAozContentBriefing*) AgentModule.Instance()->GetAgentByInternalId(AgentId.AozContentBriefing);

    public override bool HasClickableLink => true;
    public override PayloadId ClickableLinkPayloadId => PayloadId.UldahTeleport;
    
    public override bool HasTooltip => true;
    public override string TooltipText => string.Join("\n", GetIncompleteRows(Config.TaskConfig, Data.TaskData));

    public override void Load()
    {
        base.Load();
        
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "AOZContentResult", AOZContentResultPostSetup);
    }

    public override void Unload()
    {
        base.Unload();
        
        Service.AddonLifecycle.UnregisterListener(AOZContentResultPostSetup);
    }

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

    public void AOZContentResultPostSetup(AddonEvent eventType, AddonArgs addonInfo)
    {
        var addon = (AtkUnitBase*) addonInfo.Addon;
        
        if (addon->AtkValues[112] is not { Type: ValueType.UInt, UInt: var completionIndex }) throw new Exception("Type Mismatch Exception");
        if (addon->AtkValues[114] is not { Type: ValueType.Bool, Byte: var completionStatus }) throw new Exception("Type Mismatch Exception");
        
        var addonId = completionIndex switch
        {
            0 => 12449,
            1 => 12448,
            2 => 12447,

            _ => throw new ArgumentOutOfRangeException()
        };

        var task = Data.TaskData.FirstOrDefault(task => task.RowId == addonId);

        if (task is not null && task.Complete != (completionStatus != 0))
        {
            task.Complete = (completionStatus != 0);
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

    protected override ModuleStatus GetModuleStatus() => GetIncompleteCount(Config.TaskConfig, Data.TaskData) == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override StatusMessage GetStatusMessage()
    {
        var message = $"{GetIncompleteCount(Config.TaskConfig, Data.TaskData)} {Strings.ChallengesRemaining}";

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.UldahTeleport);
    }
}