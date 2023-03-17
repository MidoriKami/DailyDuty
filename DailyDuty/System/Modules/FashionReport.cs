using System;
using DailyDuty.Abstracts;
using DailyDuty.Interfaces;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using KamiLib.Misc;

namespace DailyDuty.System;

public class FashionReportConfig : ModuleConfigBase
{
    [ConfigOption("CompletionMode", "CompletionModeHelp")]
    public FashionReportMode CompletionMode = FashionReportMode.Single;

    [ClickableLink("GoldSaucerTeleport")]
    public bool ClickableLink = true;
}

public class FashionReportData : ModuleDataBase
{
    [DataDisplay("AllowancesRemaining")]
    public int AllowancesRemaining = 4;
    
    [DataDisplay("HighestWeeklyScore")]
    public int HighestWeeklyScore = 0;

    [DataDisplay("FashionReportAvailable")]
    public bool FashionReportAvailable;
}

public unsafe class FashionReport : Module.SpecialModule, IGoldSaucerMessageReceiver
{
    public override ModuleName ModuleName => ModuleName.FashionReport;

    public override ModuleConfigBase ModuleConfig { get; protected set; } = new FashionReportConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new FashionReportData();
    private FashionReportConfig Config => ModuleConfig as FashionReportConfig ?? new FashionReportConfig();
    private FashionReportData Data => ModuleData as FashionReportData ?? new FashionReportData();

    public override void Update()
    {
        var reportOpen = Time.NextWeeklyReset().AddDays(-4);
        var reportClosed = Time.NextWeeklyReset();
        var now = DateTime.UtcNow;

        Data.FashionReportAvailable = now > reportOpen && now < reportClosed;
        DataChanged = true;
        
        base.Update();
    }

    public override void Reset()
    {
        Data.HighestWeeklyScore = 0;
        Data.AllowancesRemaining = 4;
        Data.FashionReportAvailable = false;
        
        base.Reset();
    }

    public override TimeSpan GetResetPeriod() => TimeSpan.FromDays(4);
    protected override DateTime GetNextReset() => Time.NextFashionReportReset();
    protected override ModuleStatus GetModuleStatus()
    {
        if (Data.FashionReportAvailable == false) return ModuleStatus.Unavailable;

        // Zero is always "Complete"
        // Four is always "Incomplete"
        if (Data.AllowancesRemaining == 0) return ModuleStatus.Complete;
        if (Data.AllowancesRemaining == 4) return ModuleStatus.Incomplete;

        // If this line is reached, then we have between 1 and 3 remaining allowances (inclusive)
        switch (Config.CompletionMode)
        {
            case FashionReportMode.Single:
            case FashionReportMode.All when Data.AllowancesRemaining == 0:
            case FashionReportMode.Plus80 when Data.HighestWeeklyScore >= 80:
                return ModuleStatus.Complete;

            default:
                return ModuleStatus.Incomplete;
        }
    }
    
    protected override StatusMessage GetStatusMessage()
    {
        var message = Config.CompletionMode switch
        {
            FashionReportMode.All => $"{Data.AllowancesRemaining} Allowances Available",
            FashionReportMode.Single when Data.AllowancesRemaining == 4 => $"{Data.AllowancesRemaining} Allowances Available",
            FashionReportMode.Plus80 when Data.HighestWeeklyScore <= 80 => $"{Data.HighestWeeklyScore} Highest Score",
            _ => throw new ArgumentOutOfRangeException()
        };

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.GoldSaucerTeleport);
    }
    
    public void GoldSaucerUpdate(object? sender, GoldSaucerEventArgs data)
    {
        const int maskedRoseId = 1025176;
        if (Service.TargetManager.Target?.DataId != maskedRoseId) return;

        var allowances = Data.AllowancesRemaining;
        var score = Data.HighestWeeklyScore;

        switch (data.EventID)
        {
            case 5:     // When speaking to Masked Rose, gets update information
                allowances = data.Data[1];
                score = data.Data[0];
                break;

            case 3:     // During turn in, gets new score
                score = data.Data[0];
                break;
                    
            case 1:     // During turn in, gets new allowances
                allowances = data.Data[0];
                break;
        }

        if (Data.AllowancesRemaining != allowances)
        {
            Data.AllowancesRemaining = allowances;
            DataChanged = true;
        }

        if (Data.HighestWeeklyScore != score)
        {
            Data.HighestWeeklyScore = score;
            DataChanged = true;
        }
    }
}