using System;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiLib.Misc;

namespace DailyDuty.System;

public class LevequestConfig : ModuleConfigBase
{
    [ConfigOption("NotificationThreshold", 0, 100)]
    public int NotificationThreshold = 95;

    [ConfigOption("ComparisonMode", "ComparisonHelp")]
    public ComparisonMode ComparisonMode = ComparisonMode.EqualTo;
}

public class LevequestData : ModuleDataBase
{
    [DataDisplay("LevequestAllowances")]
    public int NumLevequestAllowances;

    [DataDisplay("AcceptedLevequests")] 
    public int AcceptedLevequests;
}

public unsafe class Levequest : Module.SpecialModule
{
    public override ModuleName ModuleName => ModuleName.Levequest;

    public override ModuleConfigBase ModuleConfig { get; protected set; } = new LevequestConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new LevequestData();
    private LevequestConfig Config => ModuleConfig as LevequestConfig ?? new LevequestConfig();
    private LevequestData Data => ModuleData as LevequestData ?? new LevequestData();
    
    public override TimeSpan GetResetPeriod() => TimeSpan.FromHours(12);
    protected override DateTime GetNextReset() => Time.NextLeveAllowanceReset();
    public override void Update()
    {
        TryUpdateData(ref Data.NumLevequestAllowances, QuestManager.Instance()->NumLeveAllowances);
        TryUpdateData(ref Data.AcceptedLevequests, QuestManager.Instance()->NumAcceptedLeveQuests);
        
        base.Update();
    }

    protected override ModuleStatus GetModuleStatus() => Config.ComparisonMode switch
    {
        ComparisonMode.LessThan when Config.NotificationThreshold > Data.NumLevequestAllowances => ModuleStatus.Complete,
        ComparisonMode.EqualTo when Config.NotificationThreshold == Data.NumLevequestAllowances => ModuleStatus.Complete,
        ComparisonMode.LessThanOrEqual when Config.NotificationThreshold >= Data.NumLevequestAllowances => ModuleStatus.Complete,
        _ => ModuleStatus.Incomplete
    };

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = $"{Data.NumLevequestAllowances} Allowances Remaining",
    };
}