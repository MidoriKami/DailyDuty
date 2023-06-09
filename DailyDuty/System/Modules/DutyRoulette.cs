using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using DailyDuty.System.Localization;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.AutomaticUserInterface;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class DutyRouletteConfig : ModuleTaskConfigBase<ContentRoulette>
{
    [BoolConfigOption("CompleteWhenTomeCapped", "ModuleConfiguration", 1, "CompleteWhenTomeCappedHelp")]
    public bool CompleteWhenCapped = false;
    
    [BoolDescriptionConfigOption("Enable", "ClickableLink", 4, "DutyRouletteOpenDutyFinder")]
    public bool ClickableLink = true;
}

public class DutyRouletteData : ModuleTaskDataBase<ContentRoulette>
{
    [IntDisplay("CurrentWeeklyTomestones", "ModuleData", 1)] 
    public int ExpertTomestones;

    [IntDisplay("WeeklyTomestoneLimit", "ModuleData", 1)]
    public int ExpertTomestoneCap;

    [BoolDisplay("AtWeeklyTomestoneLimit", "ModuleData", 1)]
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