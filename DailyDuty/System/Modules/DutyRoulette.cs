using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using DailyDuty.System.Localization;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class DutyRouletteConfig : ModuleConfigBase
{
    [SelectableTasks]
    public LuminaTaskConfigList<ContentRoulette> TaskConfig = new();

    [ClickableLink("DutyRouletteOpenDutyFinder")]
    public bool ClickableLink = true;

    [ConfigOption("CompleteWhenTomeCapped", "CompleteWhenTomeCappedHelp")]
    public bool CompleteWhenCapped = false;
}

public class DutyRouletteData : ModuleDataBase
{
    [SelectableTasks]
    public LuminaTaskDataList<ContentRoulette> TaskData = new();

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

    protected override void UpdateTaskLists()
    {
        var luminaUpdater = new LuminaTaskUpdater<ContentRoulette>(this, roulette => roulette.DutyType.RawString != string.Empty);
        luminaUpdater.UpdateConfig(Config.TaskConfig);
        luminaUpdater.UpdateData(Data.TaskData);
    }

    public override void Update()
    {
        Data.TaskData.Update(ref DataChanged, rowId => RouletteController.Instance()->IsRouletteComplete((byte) rowId));

        TryUpdateData(ref Data.ExpertTomestones, InventoryManager.Instance()->GetWeeklyAcquiredTomestoneCount());
        TryUpdateData(ref Data.ExpertTomestoneCap, InventoryManager.GetLimitedTomestoneWeeklyLimit());
        TryUpdateData(ref Data.AtTomeCap, Data.ExpertTomestones == Data.ExpertTomestoneCap);
        
        base.Update();
    }

    public override void Reset()
    {
        Data.TaskData.Reset();
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus()
    {
        if (Config.CompleteWhenCapped && Data.AtTomeCap) return ModuleStatus.Complete;

        return GetIncompleteCount(Config.TaskConfig, Data.TaskData) == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }
    
    protected override StatusMessage GetStatusMessage()
    {
        var message = $"{GetIncompleteCount(Config.TaskConfig, Data.TaskData)} {Strings.RoulettesRemaining}";

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.OpenDutyFinderRoulette);
    }
}