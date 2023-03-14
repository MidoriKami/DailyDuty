using System.Collections.Generic;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using DailyDuty.System.Localization;
using DailyDuty.Views.Components;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using ClientStructs = FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace DailyDuty.System;

public class ChallengeLogConfig : ModuleConfigBase
{
    public List<LuminaTaskConfig> Tasks = new();
}

public class ChallengeLogData : ModuleDataBase
{
    public List<LuminaTaskData> Tasks = new();
}

public unsafe class ChallengeLog : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.ChallengeLog;
    
    public override ModuleDataBase ModuleData { get; protected set; } = new ChallengeLogData();
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new ChallengeLogConfig();
    private ChallengeLogData Data => ModuleData as ChallengeLogData ?? new ChallengeLogData();
    private ChallengeLogConfig Config => ModuleConfig as ChallengeLogConfig ?? new ChallengeLogConfig();

    public override void Load()
    {
        base.Load();

        var luminaUpdater = new LuminaTaskUpdater<ContentsNote>(this, (row) => row.RequiredAmount is not 0);
        luminaUpdater.UpdateConfig(Config.Tasks);
        luminaUpdater.UpdateData(Data.Tasks);
    }
    
    public override void Update()
    {
        foreach (var task in Data.Tasks)
        {
            task.Complete = ClientStructs.ContentsNote.Instance()->IsContentNoteComplete((int) task.RowId);
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

    protected override ModuleStatus GetModuleStatus() => GetIncompleteCount() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    
    public override StatusMessage GetStatusMessage() => new()
    {
        Message = $"{GetIncompleteCount()} {Strings.TasksIncomplete}",
    };
    
    private string GetContentsNoteString(ContentsNote note) => note.Name.ToDalamudString().ToString();

    public override void DrawExtraConfig()
    {
        LuminaListConfigView.Draw<ContentsNote>(this, Config.Tasks, GetContentsNoteString);
    }
    
    public override void DrawExtraData()
    {
        LuminaListDataView.Draw<ContentsNote>(Data.Tasks, Config.Tasks, GetContentsNoteString);
    }

    private int GetIncompleteCount()
    {
        var enabledTasks = Config.Tasks.Where(task => task.Enabled);
        var taskData = Data.Tasks
            .Where(task => !task.Complete)
            .Where(task => enabledTasks.Any(config => config.RowId == task.RowId));

        return taskData.Count();
    }
}