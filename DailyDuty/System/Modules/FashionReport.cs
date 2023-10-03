using System;
using DailyDuty.Abstracts;
using DailyDuty.Interfaces;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.Models.ModuleData;
using DailyDuty.System.Localization;
using KamiLib.Game;

namespace DailyDuty.System;

public unsafe class FashionReport : Module.SpecialModule, IGoldSaucerMessageReceiver
{
    public override ModuleName ModuleName => ModuleName.FashionReport;

    public override IModuleConfigBase ModuleConfig { get; protected set; } = new FashionReportConfig();
    public override IModuleDataBase ModuleData { get; protected set; } = new FashionReportData();
    private FashionReportConfig Config => ModuleConfig as FashionReportConfig ?? new FashionReportConfig();
    private FashionReportData Data => ModuleData as FashionReportData ?? new FashionReportData();

    public override bool HasClickableLink => true;
    public override PayloadId ClickableLinkPayloadId => PayloadId.GoldSaucerTeleport;

    public override void Update()
    {
        var reportOpen = Time.NextWeeklyReset().AddDays(-4);
        var reportClosed = Time.NextWeeklyReset();
        var now = DateTime.UtcNow;

        Data.FashionReportAvailable = TryUpdateData(Data.FashionReportAvailable, now > reportOpen && now < reportClosed);
        
        base.Update();
    }

    public override void Reset()
    {
        Data.HighestWeeklyScore = 0;
        Data.AllowancesRemaining = 4;
        Data.FashionReportAvailable = false;
        
        base.Reset();
    }

    protected override DateTime GetNextReset() => Time.NextFashionReportReset();
    protected override ModuleStatus GetModuleStatus()
    {
        if (Data.FashionReportAvailable == false) return ModuleStatus.Unavailable;

        return Config.CompletionMode switch
        {
            FashionReportMode.Single when Data.AllowancesRemaining < 4 => ModuleStatus.Complete,
            FashionReportMode.All when Data.AllowancesRemaining is 0 => ModuleStatus.Complete,
            FashionReportMode.Plus80 when Data.HighestWeeklyScore >= 80 => ModuleStatus.Complete,
            _ => ModuleStatus.Incomplete
        };
    }

    protected override StatusMessage GetStatusMessage() => ConditionalStatusMessage.GetMessage(
        Config.ClickableLink,
        Config.CompletionMode switch
        {
            FashionReportMode.All => $"{Data.AllowancesRemaining} {Strings.AllowancesAvailable}",
            FashionReportMode.Single when Data.AllowancesRemaining == 4 => $"{Data.AllowancesRemaining} {Strings.AllowancesAvailable}",
            FashionReportMode.Plus80 when Data.HighestWeeklyScore <= 80 => $"{Data.HighestWeeklyScore} {Strings.HighestScore}",
            _ => throw new ArgumentOutOfRangeException()
        },
        PayloadId.GoldSaucerTeleport
    );
    
    public void GoldSaucerUpdate(object? sender, GoldSaucerEventArgs data)
    {
        const int maskedRoseId = 1025176;
        if (Service.TargetManager.Target?.DataId != maskedRoseId) return;

        var allowances = Data.AllowancesRemaining;
        var score = Data.HighestWeeklyScore;

        switch (data.EventId)
        {
            case 5:     // When speaking to Masked Rose, gets update information
                allowances = data.Data[1];
                score = data.Data[0];
                break;

            case 3:     // During turn in, gets new score
                score = Math.Max(data.Data[0], score);
                break;
                    
            case 1:     // During turn in, gets new allowances
                allowances = data.Data[0];
                break;
        }
        
        Data.AllowancesRemaining = TryUpdateData(Data.AllowancesRemaining, allowances);
        Data.HighestWeeklyScore = TryUpdateData(Data.HighestWeeklyScore, score);
    }
}