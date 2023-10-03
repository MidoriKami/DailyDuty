using System;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.Models.ModuleData;
using DailyDuty.System.Localization;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiLib.Game;

namespace DailyDuty.System;




public unsafe class Levequest : Module.SpecialModule
{
    public override ModuleName ModuleName => ModuleName.Levequest;

    public override IModuleConfigBase ModuleConfig { get; protected set; } = new LevequestConfig();
    public override IModuleDataBase ModuleData { get; protected set; } = new LevequestData();
    private LevequestConfig Config => ModuleConfig as LevequestConfig ?? new LevequestConfig();
    private LevequestData Data => ModuleData as LevequestData ?? new LevequestData();
    
    protected override DateTime GetNextReset() => Time.NextLeveAllowanceReset();
    public override void Update()
    {
        Data.NumLevequestAllowances = TryUpdateData(Data.NumLevequestAllowances, QuestManager.Instance()->NumLeveAllowances);
        Data.AcceptedLevequests = TryUpdateData(Data.AcceptedLevequests, QuestManager.Instance()->NumAcceptedLeveQuests);
        
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
        Message = $"{Data.NumLevequestAllowances} {Strings.AllowancesRemaining}",
    };
}