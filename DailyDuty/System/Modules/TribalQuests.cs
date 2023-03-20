using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace DailyDuty.System;

public class TribalQuestsConfig : ModuleConfigBase
{
    [ConfigOption("NotificationThreshold", 0, 12)]
    public int NotificationThreshold = 12;

    [ConfigOption("ComparisonMode", "ComparisonHelp")]
    public ComparisonMode ComparisonMode = ComparisonMode.LessThan;
}

public class TribalQuestsData : ModuleDataBase
{
    [DataDisplay("RemainingAllowances")]
    public uint RemainingAllowances;
}

public unsafe class TribalQuests : Module.DailyModule
{
    public override ModuleName ModuleName => ModuleName.TribalQuests;

    public override ModuleConfigBase ModuleConfig { get; protected set; } = new TribalQuestsConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new TribalQuestsData();
    private TribalQuestsConfig Config => ModuleConfig as TribalQuestsConfig ?? new TribalQuestsConfig();
    private TribalQuestsData Data => ModuleData as TribalQuestsData ?? new TribalQuestsData();

    public override void Update()
    {
        var allowances = QuestManager.Instance()->GetBeastTribeAllowance();

        if (Data.RemainingAllowances != allowances)
        {
            Data.RemainingAllowances = allowances;
            DataChanged = true;
        }
        
        base.Update();
    }

    public override void Reset()
    {
        Data.RemainingAllowances = 12;
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus() => Config.ComparisonMode switch
    {
        ComparisonMode.LessThan when Config.NotificationThreshold > Data.RemainingAllowances => ModuleStatus.Complete,
        ComparisonMode.EqualTo when Config.NotificationThreshold == Data.RemainingAllowances => ModuleStatus.Complete,
        ComparisonMode.LessThanOrEqual when Config.NotificationThreshold >= Data.RemainingAllowances => ModuleStatus.Complete,
        _ => ModuleStatus.Incomplete
    };
    
    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = $"{Data.RemainingAllowances} Allowances Remaining",
    };
}