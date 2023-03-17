using System.Collections.Generic;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class DutyRouletteConfig : ModuleConfigBase
{
    [SelectableTasks]
    public List<LuminaTaskConfig<ContentRoulette>> Tasks = new();

    [ClickableLink("DutyRouletteOpenDutyFinder")]
    public bool ClickableLink = true;

    [ConfigOption("CompleteWhenTomeCapped")]
    public bool CompleteWhenCapped = false;
}

public class DutyRouletteData : ModuleDataBase
{
    [SelectableTasks]
    public List<LuminaTaskData<ContentRoulette>> Tasks = new();

    [DataDisplay("CurrentWeeklyTomestones")] 
    public int ExpertTomestones;

    [DataDisplay("WeeklyTomestoneLimit")]
    public int ExpertTomestoneCap;

    [DataDisplay("AtWeeklyTomestoneLimit")]
    public bool AtTomeCap;
}

public unsafe class DutyRoulette : Module.DailyModule
{
    public override ModuleName ModuleName => ModuleName.DutyRoulette;
    
    public override ModuleDataBase ModuleData { get; protected set; } = new DutyRouletteData();
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new DutyRouletteConfig();
    private DutyRouletteData Data => ModuleData as DutyRouletteData ?? new DutyRouletteData();
    private DutyRouletteConfig Config => ModuleConfig as DutyRouletteConfig ?? new DutyRouletteConfig();
    public override void Load()
    {
        base.Load();

        var luminaUpdater = new LuminaTaskUpdater<ContentRoulette>(this, roulette => roulette.DutyType.RawString != string.Empty);
        luminaUpdater.UpdateConfig(Config.Tasks);
        luminaUpdater.UpdateData(Data.Tasks);
    }

    public override void Update()
    {
        var weeklyAcquiredTomestones = InventoryManager.Instance()->GetWeeklyAcquiredTomestoneCount();
        var weeklyTomestoneLimit = InventoryManager.GetLimitedTomestoneWeeklyLimit();
        var atWeeklyLimit = weeklyAcquiredTomestones == weeklyTomestoneLimit;
        
        foreach (var task in Data.Tasks)
        {
            var status = RouletteController.Instance()->IsRouletteComplete((byte) task.RowId);

            if (task.Complete != status)
            {
                task.Complete = status;
                DataChanged = true;
            }
        }

        if (Data.ExpertTomestones != weeklyAcquiredTomestones)
        {
            Data.ExpertTomestones = weeklyAcquiredTomestones;
            DataChanged = true;
        }

        if (Data.ExpertTomestoneCap != weeklyTomestoneLimit)
        {
            Data.ExpertTomestoneCap = weeklyTomestoneLimit;
            DataChanged = true;
        }

        if (Data.AtTomeCap != atWeeklyLimit)
        {
            Data.AtTomeCap = atWeeklyLimit;
            DataChanged = true;
        }
        
        base.Update();
    }

    public override void Reset()
    {
        foreach (var task in Data.Tasks)
        {
            task.Complete = false;
        }
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus()
    {
        if (Config.CompleteWhenCapped && Data.AtTomeCap) return ModuleStatus.Complete;

        return GetIncompleteCount() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }
    
    protected override StatusMessage GetStatusMessage()
    {
        var message = $"{GetIncompleteCount()} Roulettes Remaining";

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.OpenDutyFinderRoulette);
    }
    
    private int GetIncompleteCount()
    {
        var taskData = from config in Config.Tasks
            join data in Data.Tasks on config.RowId equals data.RowId
            where config.Enabled
            where !data.Complete
            select new
            {
                config.RowId,
                config.Enabled,
                data.Complete
            };

        return taskData.Count();
    }
}