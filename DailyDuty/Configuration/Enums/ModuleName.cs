using System;
using DailyDuty.Localization;

namespace DailyDuty.Configuration.Enums;

public enum ModuleName
{
    BeastTribe,
    CustomDelivery,
    DomanEnclave,
    DutyRoulette,
    FashionReport,
    HuntMarksDaily,
    HuntMarksWeekly,
    JumboCactpot,
    Levequest,
    MiniCactpot,
    TreasureMap,
    WondrousTails,
    Daily,
    Weekly,
    ChallengeLog
}

public static class ModuleNameExtensions
{
    public static string GetTranslatedString(this ModuleName value)
    {
        return value switch
        {
            ModuleName.BeastTribe => Strings.Module.BeastTribe.Label,
            ModuleName.CustomDelivery => Strings.Module.CustomDelivery.Label,
            ModuleName.DomanEnclave => Strings.Module.DomanEnclave.Label,
            ModuleName.DutyRoulette => Strings.Module.DutyRoulette.Label,
            ModuleName.FashionReport => Strings.Module.FashionReport.Label,
            ModuleName.HuntMarksDaily => Strings.Module.HuntMarks.DailyLabel,
            ModuleName.HuntMarksWeekly => Strings.Module.HuntMarks.WeeklyLabel,
            ModuleName.JumboCactpot => Strings.Module.JumboCactpot.Label,
            ModuleName.Levequest => Strings.Module.Levequest.Label,
            ModuleName.MiniCactpot => Strings.Module.MiniCactpot.Label,
            ModuleName.TreasureMap => Strings.Module.TreasureMap.Label,
            ModuleName.WondrousTails => Strings.Module.WondrousTails.Label,
            ModuleName.Daily => Strings.Common.Daily,
            ModuleName.Weekly => Strings.Common.Weekly,
            ModuleName.ChallengeLog => Strings.Module.ChallengeLog.Label,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}