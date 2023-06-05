using System;
using DailyDuty.Abstracts;
using DailyDuty.Interfaces;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using KamiLib.AutomaticUserInterface;
using KamiLib.Utilities;

namespace DailyDuty.System;

public class FashionReportConfig : ModuleConfigBase
{
    [DrawCategory("ModuleConfiguration", 1)]
    [EnumConfigOption("CompletionMode")]
    public FashionReportMode CompletionMode = FashionReportMode.Single;
    
    [DrawCategory("ClickableLink", 2)]
    [BoolDescriptionConfigOption("Enable", "GoldSaucerTeleport")] 
    public bool ClickableLink = true;
}

public class FashionReportData : ModuleDataBase
{
    [DrawCategory("ModuleData", 1)]
    [IntDisplay("AllowancesRemaining")]
    public int AllowancesRemaining = 4;
    
    [DrawCategory("ModuleData", 1)]
    [IntDisplay("HighestWeeklyScore")]
    public int HighestWeeklyScore;

    [DrawCategory("ModuleData", 1)]
    [BoolDisplay("FashionReportAvailable")]
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

        TryUpdateData(ref Data.FashionReportAvailable, now > reportOpen && now < reportClosed);
        
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
                score = data.Data[0];
                break;
                    
            case 1:     // During turn in, gets new allowances
                allowances = data.Data[0];
                break;
        }
        
        TryUpdateData(ref Data.AllowancesRemaining, allowances);
        TryUpdateData(ref Data.HighestWeeklyScore, score);
    }
}