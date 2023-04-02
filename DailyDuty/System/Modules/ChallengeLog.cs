using System.Collections.Generic;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using DailyDuty.System.Localization;
using Lumina.Excel.GeneratedSheets;
using ClientStructs = FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace DailyDuty.System;

public class ChallengeLogConfig : ModuleConfigBase
{
    [SelectableTasks]
    public LuminaTaskConfigList<ContentsNote> TaskConfig = new();
}

public class ChallengeLogData : ModuleDataBase
{
    [SelectableTasks] 
    public LuminaTaskDataList<ContentsNote> TaskData = new();
}

public unsafe class ChallengeLog : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.ChallengeLog;
    
    public override ModuleDataBase ModuleData { get; protected set; } = new ChallengeLogData();
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new ChallengeLogConfig();
    private ChallengeLogData Data => ModuleData as ChallengeLogData ?? new ChallengeLogData();
    private ChallengeLogConfig Config => ModuleConfig as ChallengeLogConfig ?? new ChallengeLogConfig();
    
    protected override void UpdateTaskLists()
    {
        var luminaUpdater = new LuminaTaskUpdater<ContentsNote>(this, (row) => row.RequiredAmount is not 0);
        luminaUpdater.UpdateConfig(Config.TaskConfig);
        luminaUpdater.UpdateData(Data.TaskData);
    }

    public override void Update()
    {
        Data.TaskData.Update(ref DataChanged, rowId => ClientStructs.ContentsNote.Instance()->IsContentNoteComplete((int) rowId));

        base.Update();
    }

    public override void Reset()
    {
        Data.TaskData.Reset();
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus() => GetIncompleteCount(Config.TaskConfig, Data.TaskData) == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = $"{GetIncompleteCount(Config.TaskConfig, Data.TaskData)} {Strings.TasksIncomplete}",
    };
}