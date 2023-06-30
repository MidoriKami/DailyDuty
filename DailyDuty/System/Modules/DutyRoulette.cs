using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.Models.ModuleData;
using DailyDuty.System.Helpers;
using DailyDuty.System.Localization;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public unsafe class DutyRoulette : Module.DailyModule
{
    public override ModuleName ModuleName => ModuleName.DutyRoulette;
    
    public override IModuleDataBase ModuleData { get; protected set; } = new DutyRouletteData();
    public override IModuleConfigBase ModuleConfig { get; protected set; } = new DutyRouletteConfig();
    private DutyRouletteData Data => ModuleData as DutyRouletteData ?? new DutyRouletteData();
    private DutyRouletteConfig Config => ModuleConfig as DutyRouletteConfig ?? new DutyRouletteConfig();
    
    public override bool HasClickableLink => true;
    public override PayloadId ClickableLinkPayloadId => PayloadId.OpenDutyFinderRoulette;

    public override bool HasTooltip => true;
    public override string TooltipText => string.Join("\n", GetIncompleteRows(Config.TaskConfig, Data.TaskData));

    protected override void UpdateTaskLists()
    {
        var luminaUpdater = new LuminaTaskUpdater<ContentRoulette>(this, roulette => roulette.DutyType.RawString != string.Empty);
        luminaUpdater.UpdateConfig(Config.TaskConfig);
        luminaUpdater.UpdateData(Data.TaskData);
    }

    public override void Update()
    {
        Data.TaskData.Update(ref DataChanged, rowId => RouletteController.Instance()->IsRouletteComplete((byte) rowId));

        Data.ExpertTomestones = TryUpdateData(Data.ExpertTomestones, InventoryManager.Instance()->GetWeeklyAcquiredTomestoneCount());
        Data.ExpertTomestoneCap = TryUpdateData(Data.ExpertTomestoneCap, InventoryManager.GetLimitedTomestoneWeeklyLimit());
        Data.AtTomeCap = TryUpdateData(Data.AtTomeCap, Data.ExpertTomestones == Data.ExpertTomestoneCap);
        
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